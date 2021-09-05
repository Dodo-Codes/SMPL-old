using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TcpClient = NetCoreServer.TcpClient;
using UdpClient = NetCoreServer.UdpClient;

namespace SMPL.Gear
{
	public static class Multiplayer
	{
		private static event Events.ParamsZero OnServerStart, OnServerStop;
		private static event Events.ParamsOne<string> OnClientConnected, OnClientDisconnected, OnClientTakenUniqueID;
		private static event Events.ParamsOne<Message> OnMessageReceived, OnMessageSend;
		private static readonly Dictionary<Guid, string> clientRealIDs = new();
		private static readonly List<string> clientIDs = new();
		private static readonly int serverPort = 1234;

		private static string MessageToString(Message message)
		{
			var rel = message.IsReliable ? "1" : "0";
			return
				$"{Message.SEP}" +
				$"{(int)message.type}{Message.COMP_SEP}" +
				$"{rel}{Message.COMP_SEP}" +
				$"{message.SenderUniqueID}{Message.COMP_SEP}" +
				$"{message.ReceiverUniqueID}{Message.COMP_SEP}" +
				$"{(int)message.Receivers}{Message.COMP_SEP}" +
				$"{message.Tag}{Message.COMP_SEP}" +
				$"{message.Content}";
		}
		private static List<Message> StringToMessages(string message)
		{
			var result = new List<Message>();
			var split = message.Split(Message.SEP, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < split.Length; i++)
			{
				var comps = split[i].Split(Message.COMP_SEP);
				result.Add(new Message()
				{
					type = (Message.Type)int.Parse(comps[0]),
					IsReliable = comps[1] == "1",
					SenderUniqueID = comps[2],
					ReceiverUniqueID = comps[3],
					Receivers = (Message.Toward)int.Parse(comps[4]),
					Tag = comps[5],
					Content = comps[6]
				});
			}
			return result;
		}
		private static bool MessageDisconnected()
		{
			if (ClientIsConnected == false && ServerIsRunning == false)
			{
				if (MessagesAreLogged == false) return true;
				Console.Log("Cannot send a message while disconnected.\n");
				return true;
			}
			return false;
		}
		private static void LogMessage(Message msg, bool send)
		{
			if (MessagesAreLogged == false || Debug.IsActive == false) return;
			var debugStr = Debug.IsActive ? Debug.debugString : "";
			Console.Log((send ? "SENT " : "RECEIVED ") + $"({debugStr})");
			Console.Log($"{msg}\n");
		}
		private static string ConnectedClients() => $"Connected clients: {clientIDs.Count}.";
		private static void DecodeMessages(Guid sessionID, string rawMessages)
		{
			var messages = StringToMessages(rawMessages);
			if (ServerIsRunning)
			{
				var messageBack = "";
				for (int i = 0; i < messages.Count; i++)
				{
					var msg = messages[i];
					switch (msg.type)
					{
						case Message.Type.Connection: // A client just connected and sent his ID & unique name
							{
								if (clientIDs.Contains(msg.SenderUniqueID)) // Is the unique name free?
								{
									msg.SenderUniqueID = ChangeID(msg.SenderUniqueID);
									// Send a message back with a free one toward the same ID so the client can recognize it's for him
									var freeUidMsg = new Message(
										Message.Toward.Client, null, msg.Content, true, msg.SenderUniqueID)
									{ type = Message.Type.ChangeID };
									messageBack += MessageToString(freeUidMsg);

									string ChangeID(string ID)
									{
										var i = 0;
										while (true)
										{
											i++;
											if (clientIDs.Contains(ID + i) == false) break;
										}
										return $"{ID}{i}";
									}
								}
								clientRealIDs[sessionID] = msg.SenderUniqueID;
								clientIDs.Add(msg.SenderUniqueID);

								// Sticking another message to update the newcoming client about online clients
								var onlineMsg = new Message(Message.Toward.Client, null, null, true, msg.SenderUniqueID)
								{ type = Message.Type.ClientOnline };
								for (int j = 0; j < clientIDs.Count; j++)
									onlineMsg.Content += $"{Message.TEMP_SEP}{clientIDs[j]}";
								messageBack += MessageToString(onlineMsg);

								// Sticking a third message to update online clients about the newcomer.
								var newComMsg = new Message(Message.Toward.AllClients, null, msg.SenderUniqueID)
								{ type = Message.Type.ClientConnected };
								messageBack += MessageToString(newComMsg);
								Console.Log($"Client '{msg.SenderUniqueID}' connected. {ConnectedClients()}\n");
								OnClientConnected?.Invoke(msg.SenderUniqueID);
								break;
							}
						case Message.Type.ClientToAll: // A client wants to send a message to everyone
							{
								messageBack += MessageToString(msg);
								break;
							}
						case Message.Type.ClientToClient: // A client wants to send a message to another client
							{
								messageBack += MessageToString(msg);
								break;
							}
						case Message.Type.ClientToServer: // A client sent me (the server) a message
							{
								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case Message.Type.ClientToAllAndServer: // A client is sending me (the server) and all other clients a message
							{
								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);

								messageBack += MessageToString(msg);
								break;
							}
					}
				}
				if (messageBack != "") server.Multicast(messageBack);
			}
			else
			{
				for (int i = 0; i < messages.Count; i++)
				{
					var msg = messages[i];
					switch (msg.type)
					{
						case Message.Type.ChangeID: // Server said someone's ID is taken and sent a free one
							{
								if (msg.Content == sessionID.ToString()) // Is this for me? (UID is still old so ID check)
								{
									var oldID = ClientUniqueID;
									var newID = msg.ReceiverUniqueID;
									clientIDs.Remove(oldID);
									clientIDs.Add(newID);
									ClientUniqueID = newID;

									Console.Log($"Client Unique ID '{oldID}' is taken. New Client Unique ID is '{newID}'.\n");
									OnClientTakenUniqueID?.Invoke(oldID);
								}
								break;
							}
						case Message.Type.ClientConnected: // Server said some client connected
							{
								if (msg.Content != ClientUniqueID) // If not me
								{
									clientIDs.Add(msg.Content);
									Console.Log($"Client '{msg.Content}' connected. {ConnectedClients()}\n");
									OnClientConnected?.Invoke(msg.Content);
								}
								// when it's me it's handled in Client.OnConnected overriden method
								break;
							}
						case Message.Type.ClientDisconnected: // Server said some client disconnected
							{
								clientIDs.Remove(msg.Content);
								Console.Log($"Client '{msg.Content}' disconnected. {ConnectedClients()}\n");
								OnClientDisconnected?.Invoke(msg.Content);
								break;
							}
						case Message.Type.ClientOnline: // Someone just connected and is getting updated on who is already online
							{
								if (msg.ReceiverUniqueID != ClientUniqueID) break; // Not for me? Not interested.

								var clientUIDs = msg.Content.Split(Message.TEMP_SEP, StringSplitOptions.RemoveEmptyEntries);
								for (int j = 0; j < clientUIDs.Length; j++)
								{
									if (clientIDs.Contains(clientUIDs[j])) continue;
									clientIDs.Add(clientUIDs[j]);
								}
								Console.Log($"{ConnectedClients()}\n");
								break;
							}
						case Message.Type.ClientToAll: // A client is sending a message to all clients
							{
								if (msg.SenderUniqueID == ClientUniqueID) break; // Is this my message coming back to me?
								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case Message.Type.ClientToAllAndServer: // A client is sending a message to the server and all clients
							{
								if (msg.SenderUniqueID == ClientUniqueID) break; // Is this my message coming back to me?
								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case Message.Type.ClientToClient: // A client is sending a message to another client
							{
								if (msg.ReceiverUniqueID != ClientUniqueID) break; // Not for me? Not interested.
								if (msg.SenderUniqueID == ClientUniqueID) return; // Is this my message coming back to me? (unlikely)
								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case Message.Type.ServerToAll: // The server sent everyone a message
							{
								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case Message.Type.ServerToClient: // The server sent some client a message
							{
								if (msg.ReceiverUniqueID != ClientUniqueID) return; // Not for me?
								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
					}
				}
			}
		}

		//=================

		internal class Session : TcpSession
		{
			public Session(TcpServer server) : base(server) { }

			protected override void OnConnected() { }
			protected override void OnDisconnected()
			{
				var disconnectedClient = clientRealIDs[Id];
				clientRealIDs.Remove(Id);
				clientIDs.Remove(disconnectedClient);
				var msg = new Message(Message.Toward.AllClients, null, disconnectedClient)
				{ type = Message.Type.ClientDisconnected };
				SendMessage(msg);

				Console.Log($"Client '{disconnectedClient}' disconnected. {ConnectedClients()}\n");
				OnClientDisconnected?.Invoke(disconnectedClient);
			}
			protected override void OnReceived(byte[] buffer, long offset, long size)
			{
				var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
				DecodeMessages(Id, rawMessages);
			}
			protected override void OnError(SocketError error) => Debug.LogError(-1, $"{error}", true);
		}
		internal class Server : TcpServer
		{
			public Server(IPAddress address, int port) : base(address, port) { }
			protected override TcpSession CreateSession() { return new Session(this); }
			protected override void OnError(SocketError error)
			{
				ServerIsRunning = false;
				Debug.LogError(-1, $"{error}", true);
				OnServerStop?.Invoke();
			}
		}
		internal class Client : TcpClient
		{
			private bool stop;

			public Client(string address, int port) : base(address, port) { }

			public void DisconnectAndStop()
			{
				stop = true;
				DisconnectAsync();
				while (IsConnected) Thread.Yield();
			}
			protected override void OnConnected()
			{
				ClientIsConnected = true;
				clientIDs.Add(ClientUniqueID);
				Console.Log($"Connected as '{ClientUniqueID}' to {Window.Title} LAN Server {client.Socket.RemoteEndPoint}.\n");

				OnClientConnected?.Invoke(ClientUniqueID);

				var msg = new Message(Message.Toward.Server, null, Id.ToString()) { type = Message.Type.Connection };
				client.SendAsync(MessageToString(msg));
			}
			protected override void OnDisconnected()
			{
				if (ClientIsConnected)
				{
					ClientIsConnected = false;
					Console.Log("Disconnected from the Server.\n");
					clientIDs.Clear();
					OnClientDisconnected?.Invoke(ClientUniqueID);
					if (stop == true) return;
				}

				// Wait for a while...
				Thread.Sleep(1000);

				// Try to connect again
				Console.Log("Trying to reconnect...\n");
				ConnectAsync();
			}
			protected override void OnReceived(byte[] buffer, long offset, long size)
			{
				var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
				DecodeMessages(Id, rawMessages);
			}
			protected override void OnError(SocketError error)
			{
				ClientIsConnected = false;
				Debug.LogError(-1, $"{error}", true);
			}
		}
		internal class MulticastClient : UdpClient
		{
			public string Multicast;
			private bool stop;

			public MulticastClient(string address, int port) : base(address, port) { }

			public void DisconnectAndStop()
			{
				stop = true;
				Disconnect();
				while (IsConnected)
					Thread.Yield();
			}
			protected override void OnConnected()
			{
				JoinMulticastGroup(Multicast); // Join UDP multicast group
				ReceiveAsync(); // Start receive datagrams
			}
			protected override void OnDisconnected()
			{
				Thread.Sleep(1000); // Wait for a while...

				// Try to connect again
				if (stop == false) Connect();
			}
			protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
			{
				var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
				DecodeMessages(Id, rawMessages);

				// Continue receive datagrams
				ReceiveAsync();
			}
			protected override void OnError(SocketError error) => Debug.LogError(-1, $"{error}", true);
		}

		internal static MulticastClient multicastClient;
		internal static Server server;
		internal static Client client;

		//=================

		public struct Message
		{
			internal enum Type
			{
				None, Connection, ChangeID, ClientConnected, ClientDisconnected, ClientOnline,
				ClientToAll, ClientToClient, ClientToServer, ServerToAll, ServerToClient,
				ClientToAllAndServer
			}
			public enum Toward { Server, Client, AllClients, ServerAndAllClients }
			internal const string SEP = "';qi#ou3", COMP_SEP = "a;@lsfi", TEMP_SEP = "a15`g&";

			public string Content { get; set; }
			public string Tag { get; set; }
			public string ReceiverUniqueID { get; set; }
			public string SenderUniqueID { get; internal set; }
			public Toward Receivers { get; set; }
			public bool IsReliable { get; set; }
			internal Type type;

			public Message(Toward receivers, string tag, string content, bool isReliable = true,
				string receiverClientUniqueID = null)
         {
				Content = content;
				Tag = tag;
				ReceiverUniqueID = receiverClientUniqueID;
				SenderUniqueID = ClientUniqueID;
				Receivers = receivers;
				IsReliable = isReliable;
				type = receivers switch
				{
					Toward.Server => ClientIsConnected ? Type.ClientToServer : Type.None,
					Toward.Client => ClientIsConnected ? Type.ClientToClient : Type.ServerToClient,
					Toward.AllClients => ClientIsConnected ? Type.ClientToAll : Type.ServerToAll,
					Toward.ServerAndAllClients => ClientIsConnected ? Type.ClientToAllAndServer : Type.ServerToAll,
					_ => Type.None,
				};
			}

         public override string ToString()
         {
				var send = SenderUniqueID == null || SenderUniqueID == "" ? "from the Server" : $"from Client '{SenderUniqueID}'";
				var rel = IsReliable ? "Reliable" : "Unreliable";
				var rec = Receivers == Toward.Client ?
					$"to Client '{ReceiverUniqueID}'" : $"to {Receivers}";
				return
					$"{rel} Multiplayer Message {send} {rec}\n" +
					$"Tag: {Tag}\n" +
					$"Content: {Content}";
         }
      }
		public static class CallWhen
		{
			public static void ServerStart(Action method, uint order = uint.MaxValue) =>
			OnServerStart = Events.Add(OnServerStart, method, order);
			public static void ServerStop(Action method, uint order = uint.MaxValue) =>
				OnServerStop = Events.Add(OnServerStop, method, order);
			public static void ClientConnect(Action<string> method, uint order = uint.MaxValue) =>
				OnClientConnected = Events.Add(OnClientConnected, method, order);
			public static void ClientDisconnect(Action<string> method, uint order = uint.MaxValue) =>
				OnClientDisconnected = Events.Add(OnClientDisconnected, method, order);
			// string = oldID
			public static void ClientTakenUniqueID(Action<string> method, uint order = uint.MaxValue) =>
				OnClientTakenUniqueID = Events.Add(OnClientTakenUniqueID, method, order);
			public static void MessageReceive(Action<Message> method, uint order = uint.MaxValue) =>
				OnMessageReceived = Events.Add(OnMessageReceived, method, order);
			public static void MessageSend(Action<Message> method, uint order = uint.MaxValue) =>
				OnMessageSend = Events.Add(OnMessageSend, method, order);
		}

		public static string SameDeviceIP { get { return "127.0.0.1"; } }
		public static bool MessagesAreLogged { get; set; }
		public static bool ClientIsConnected { get; private set; }
		public static bool ServerIsRunning { get; private set; }
		public static string ClientUniqueID { get; private set; }

		public static void StartServer()
		{
			try
			{
				if (ServerIsRunning) { Debug.LogError(1, "Server is already starting/started.", true); return; }
				if (ClientIsConnected) { Debug.LogError(1, "Cannot start a Server while a Client.", true); return; }
				server = new Server(IPAddress.Any, serverPort);
				server.Start();
				ServerIsRunning = true;

				var hostName = Dns.GetHostName();
				var hostEntry = Dns.GetHostEntry(hostName);
				var connectToServerInfo =
					"Clients can connect through those IPs if they are in the same network\n" +
					"(device / router / Virtual Private Network programs like Hamachi or Radmin):\n" +
					$"Same device: {SameDeviceIP}";
				var vpnIP = "";
				var routerIP = "";

				for (int i = 0; i < hostEntry.AddressList.Length; i++)
				{
					if (hostEntry.AddressList[i].AddressFamily != AddressFamily.InterNetwork) continue;

					var ipParts = hostEntry.AddressList[i].ToString().Split('.');
					var isRouter = ipParts[0] == "192" && ipParts[1] == "168";
					var ipType = isRouter ? "Same router: " : "Same VPN: ";
					connectToServerInfo = $"{connectToServerInfo}\n{ipType}{hostEntry.AddressList[i]}";
					if (isRouter) routerIP = hostEntry.AddressList[i].ToString();
					else vpnIP = hostEntry.AddressList[i].ToString();
				}
				if (vpnIP != "") ConnectClient(null, vpnIP);
				else ConnectClient(null, routerIP);

				Console.Log($"Started a {Window.Title} LAN Server.\n{connectToServerInfo}\n");
				OnServerStart?.Invoke();
			}
			catch (Exception ex)
			{
				var se = default(SocketException);
				var msg = ex.Message;
				Statics.TryCast(ex, out se);

				ServerIsRunning = false;
				if (se.ErrorCode == 10048) msg = "Another server is already running on that IP/port.";
				Debug.LogError(1, msg, true);
				OnServerStop?.Invoke();
			}
		}
		public static void StopServer()
		{
			try
			{
				if (ServerIsRunning == false) { Debug.LogError(1, "Server is not running.\n", true); return; }
				if (ClientIsConnected) { Debug.LogError(1, "Cannot stop a server while a client.\n", true); return; }
				ServerIsRunning = false;
				server.Stop();
				Console.Log($"The {Window.Title} LAN Server was stopped.\n");
				OnServerStop?.Invoke();
			}
			catch (Exception ex)
			{
				ServerIsRunning = false;
				Debug.LogError(-1, ex.Message, true);
				OnServerStop?.Invoke();
				return;
			}
		}

		public static void ConnectClient(string clientUniqueID, string serverIP)
      {
			if (ClientIsConnected) { Debug.LogError(1, "Already connecting/connected.\n", true); return; }
			if (ServerIsRunning) { Debug.LogError(1, "Cannot connect as Client while hosting a Server.\n", true); return; }
			try
			{
				client = new Client(serverIP, serverPort);
				multicastClient = new MulticastClient(serverIP, serverPort);
			}
			catch (Exception)
			{
				Debug.LogError(1, $"The IP '{serverIP}' is invalid.\n", true);
				return;
			}
			multicastClient.SetupMulticast(true);
			multicastClient.Multicast = "239.255.0.1";
			multicastClient.Connect();
			multicastClient.Socket.EnableBroadcast = true;

			client.ConnectAsync();
			ClientUniqueID = clientUniqueID;
			ClientIsConnected = true;

			Console.Log($"Connecting to {Window.Title} Server '{serverIP}:{serverPort}'...\n");
		}
		public static void DisconnectClinet()
      {
			if (ClientIsConnected == false)
			{
				Debug.LogError(1, "Cannot disconnect when not connected as Client.\n", true);
				return;
			}
			client.DisconnectAndStop();
			multicastClient.DisconnectAndStop();
		}

		public static void SendMessage(Message message)
		{
			if (MessageDisconnected()) return;
			if (ServerIsRunning && message.Receivers == Message.Toward.Server) return;
			var msgStr = MessageToString(message);
			if (ClientIsConnected)
			{
				if (message.IsReliable) client.SendAsync(msgStr);
				else multicastClient.SendAsync(msgStr);
			}
			else
			{
				if (message.IsReliable) server.Multicast(msgStr);
				else multicastClient.SendAsync(msgStr);
			}
			LogMessage(message, true);
			OnMessageSend?.Invoke(message);
		}
	}
}
