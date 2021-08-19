using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TcpClient = NetCoreServer.TcpClient;

namespace SMPL
{
	public static class Multiplayer
	{
		private enum MessageType
		{
			Connection, ChangeID, ClientConnected, ClientDisconnected, ClientOnline,
			ClientToAll, ClientToClient, ClientToServer, ServerToAll, ServerToClient,
			ClientToAllAndServer
		}
		public enum Receivers { Server, Client, AllClients, ServerAndAllClients }
		public struct Message
		{
			public string Content { get; set; }
			public string Tag { get; set; }
			public string ReceiverClientUniqueID { get; set; }
			public string SenderClientUniqueID { get; internal set; }
			public Receivers Receivers { get; set; }

			public Message(string tag, string content, Receivers receivers,
				string receiverClientUniqueID = null)
         {
				Content = content;
				Tag = tag;
				ReceiverClientUniqueID = receiverClientUniqueID;
				SenderClientUniqueID = ClientUniqueID;
				Receivers = receivers;
         }

         public override string ToString()
         {
				var send = SenderClientUniqueID == null ? "from the Server" : $"from Client '{SenderClientUniqueID}'";
				var rec = Receivers == Receivers.Client ?
					$"to Client '{ReceiverClientUniqueID}'" : $"to {Receivers}";
				return
					$"Multiplayer Message {send} {rec}\n" +
					$"Tag: {Tag}\n" +
					$"Content: {Content}";
         }
      }

		internal static Server server;
		internal static Client client;

		private static event Events.ParamsZero OnServerStart, OnServerStop;
		private static event Events.ParamsOne<string> OnClientConnected, OnClientDisconnected, OnClientTakenUniqueID;
		private static event Events.ParamsOne<Message> OnMessageReceived, OnMessageSend;

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

		private static Dictionary<string, string> clientRealIDs = new();
		private static List<string> clientIDs = new();
		private static readonly int serverPort = 1234;
		private static readonly string msgSep = "';qi#ou3", msgCompSep = "a;@lsfi";

		public static string SameDeviceIP { get { return "127.0.0.1"; } }
		public static bool MessagesAreLogged { get; set; } = true;
		public static bool ClientIsConnected { get; private set; }
		public static bool ServerIsRunning { get; private set; }
		public static string ClientUniqueID { get; private set; }

		public static void StartServer()
		{
			try
			{
				if (ServerIsRunning)
				{
					Debug.LogError(1, "Server is already starting/started.", true);
					return;
				}
				if (ClientIsConnected)
				{
					Debug.LogError(1, "Cannot start a server while a Client.", true);
					return;
				}
				server = new Server(IPAddress.Any, serverPort);
				server.Start();
				ServerIsRunning = true;

				var hostName = Dns.GetHostName();
				var hostEntry = Dns.GetHostEntry(hostName);
				var connectToServerInfo =
					"Clients can connect through those IPs if they are in the same network\n" +
					"(device / router / Virtual Private Network programs like Hamachi or Radmin):\n" +
					$"Same device: {SameDeviceIP}";
				for (int i = 0; i < hostEntry.AddressList.Length; i++)
				{
					if (hostEntry.AddressList[i].AddressFamily != AddressFamily.InterNetwork) continue;

					var ipParts = hostEntry.AddressList[i].ToString().Split('.');
					var ipType = ipParts[0] == "192" && ipParts[1] == "168" ?
						"Same router: " : "Same VPN: ";
					connectToServerInfo = $"{connectToServerInfo}\n{ipType}{hostEntry.AddressList[i]}";
				}
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
				if (ServerIsRunning == false)
				{
					Debug.LogError(1, "Server is not running.\n", true);
					return;
				}
				if (ClientIsConnected)
				{
					Debug.LogError(1, "Cannot stop a server while a client.\n", true);
					return;
				}
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
			if (ClientIsConnected)
			{
				Debug.LogError(1, "Already connecting/connected.\n", true);
				return;
			}
			if (ServerIsRunning)
			{
				Debug.LogError(1, "Cannot connect as Client while hosting a Server.\n", true);
				return;
			}

			try
			{
				client = new Client(serverIP, serverPort);
			}
			catch (Exception)
			{
				Debug.LogError(1, $"The IP '{serverIP}' is invalid.\n", true);
				return;
			}
			ClientUniqueID = clientUniqueID;
			Console.Log($"Connecting to {Window.Title} Server '{serverIP}:{serverPort}'...\n");
			client.ConnectAsync();
		}
		public static void DisconnectClinet()
      {
			if (ClientIsConnected == false)
			{
				Debug.LogError(1, "Cannot disconnect when not connected as Client.\n", true);
				return;
			}
			client.DisconnectAndStop();
		}

		public static void SendMessage(Message message)
		{
			if (MessageDisconnected()) return;
			switch (message.Receivers)
			{
				case Receivers.Server:
					{
						if (ServerIsRunning || ClientIsConnected == false) break;

						var str = $"{msgSep}{(int)MessageType.ClientToServer}{msgCompSep}" +
							$"{ClientUniqueID}{msgCompSep}{message.Tag}{msgCompSep}{message.Content}";
						client.SendAsync(str);
						LogMessage(message, true);
						break;
					}
				case Receivers.AllClients:
					{
						if (ClientIsConnected)
						{
							client.SendAsync($"{msgSep}{(int)MessageType.ClientToAll}{msgCompSep}" +
								$"{ClientUniqueID}{msgCompSep}{message.Tag}{msgCompSep}{message.Content}");
						}
						else if (ServerIsRunning)
						{
							server.Multicast($"{msgSep}{(int)MessageType.ServerToAll}" +
								$"{msgCompSep}{message.Tag}{msgCompSep}{message.Content}");
						}
						LogMessage(message, true);
						break;
					}
				case Receivers.ServerAndAllClients:
					{
						if (ClientIsConnected)
						{
							client.SendAsync($"{msgSep}{(int)MessageType.ClientToAllAndServer}{msgCompSep}" +
								$"{ClientUniqueID}{msgCompSep}{message.Tag}{msgCompSep}{message.Content}");
						}
						else if (ServerIsRunning)
						{
							server.Multicast($"{msgSep}{(int)MessageType.ServerToAll}" +
								$"{msgCompSep}{message.Tag}{msgCompSep}{message.Content}");
						}
						LogMessage(message, true);
						break;
					}
				case Receivers.Client:
               {
						if (ClientIsConnected)
						{
							client.SendAsync($"{msgSep}{(int)MessageType.ClientToClient}{msgCompSep}" +
								$"{ClientUniqueID}{msgCompSep}{message.ReceiverClientUniqueID}" +
								$"{msgCompSep}{message.Tag}{msgCompSep}{message.Content}");
						}
						else if (ServerIsRunning)
						{
							server.Multicast($"{msgSep}{(int)MessageType.ServerToClient}{msgCompSep}" +
								$"{message.Tag}{msgCompSep}{message.Content}");
						}
						LogMessage(message, true);
						break;
					}
			}
			OnMessageSend?.Invoke(message);
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
			Console.Log((send ? "SENT " : "RECEIVED ") + debugStr);
			Console.Log($"{msg}\n");
		}
		private static string ConnectedClients() => $"Connected clients: {clientIDs.Count}.";

		internal class Session : TcpSession
		{
			public Session(TcpServer server) : base(server) { }

			protected override void OnConnected() { }
			protected override void OnDisconnected()
			{
				var disconnectedClient = clientRealIDs[Id.ToString()];
				clientIDs.Remove(disconnectedClient);
				server.Multicast($"{msgSep}{(int)MessageType.ClientDisconnected}{msgCompSep}{disconnectedClient}");

				Console.Log($"Client '{disconnectedClient}' disconnected. {ConnectedClients()}\n");
				OnClientDisconnected?.Invoke(disconnectedClient);
			}
			protected override void OnReceived(byte[] buffer, long offset, long size)
			{
				var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
				var messages = rawMessages.Split(msgSep, StringSplitOptions.RemoveEmptyEntries);
				var messageBack = "";
				for (int j = 0; j < messages.Length; j++)
				{
					var components = messages[j].Split(msgCompSep);
					var messageType = (MessageType)int.Parse(components[0]);
					switch (messageType)
					{
						case MessageType.Connection: // A client just connected and sent his ID & unique name
							{
								var id = components[1];
								var clientID = components[2];
								if (clientIDs.Contains(clientID)) // Is the unique name free?
								{
									clientID = ChangeID(clientID);
									// Send a message back with a free one toward the same ID so the client can recognize it's for him
									messageBack = $"{msgSep}{(int)MessageType.ChangeID}{msgCompSep}" +
										$"{id}{msgCompSep}{clientID}";
								}
								clientRealIDs[Id.ToString()] = clientID;
								clientIDs.Add(clientID);

								// Sticking another message to update the newcoming client about online clients
								messageBack = $"{messageBack}{msgSep}{(int)MessageType.ClientOnline}" +
									$"{msgCompSep}{clientID}";
								for (int i = 0; i < clientIDs.Count; i++)
								{
									messageBack = $"{messageBack}{msgCompSep}{clientIDs[i]}";
								}

								// Sticking a third message to update online clients about the newcomer.
								messageBack =
									$"{messageBack}{msgSep}{(int)MessageType.ClientConnected}{msgCompSep}" +
									$"{clientID}";
								Console.Log($"Client '{clientID}' connected. {ConnectedClients()}\n");
								OnClientConnected?.Invoke(clientID);
								break;
							}
						case MessageType.ClientToAll: // A client wants to send a message to everyone
							{
								messageBack = $"{messageBack}{msgSep}{messages[j]}";
								break;
							}
						case MessageType.ClientToClient: // A client wants to send a message to another client
							{
								messageBack = $"{messageBack}{msgSep}{messages[j]}";
								break;
							}
						case MessageType.ClientToServer: // A client sent me (the server) a message
							{
								var msg = new Message(components[2], components[3], Receivers.Server)
								{ SenderClientUniqueID = components[1] };

								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case MessageType.ClientToAllAndServer: // A client is sending me (the server) and all other clients a message
							{
								var msg = new Message(components[2], components[3], Receivers.Server)
								{ SenderClientUniqueID = components[1] };

								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);

								messageBack = $"{messageBack}{msgSep}{messages[j]}";
								break;
							}
					}
				}
				if (messageBack != "") server.Multicast(messageBack);
			}
			protected override void OnError(SocketError error) => Debug.LogError(-1, $"{error}", true);
			private static string ChangeID(string ID)
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

				client.SendAsync($"{msgSep}{(int)MessageType.Connection}{msgCompSep}{client.Id}{msgCompSep}{ClientUniqueID}");
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
				var messages = rawMessages.Split(msgSep, StringSplitOptions.RemoveEmptyEntries);
				var messageBack = "";
				for (int j = 0; j < messages.Length; j++)
				{
					var components = messages[j].Split(msgCompSep);
					var messageType = (MessageType)int.Parse(components[0]);
					switch (messageType)
					{
						case MessageType.ChangeID: // Server said someone's ID is taken and sent a free one
							{
								if (components[1] == client.Id.ToString()) // Is this for me?
								{
									var oldID = ClientUniqueID;
									var newID = components[2];
									clientIDs.Remove(oldID);
									clientIDs.Add(newID);

									Console.Log($"Client Unique ID '{oldID}' is taken. New Client Unique ID is '{newID}'.\n");
									OnClientTakenUniqueID?.Invoke(oldID);
								}
								break;
							}
						case MessageType.ClientConnected: // Server said some client connected
							{
								var ID = components[1];
								if (ID != ClientUniqueID) // If not me
								{
									clientIDs.Add(ID);
									Console.Log($"Client '{ID}' connected. {ConnectedClients()}\n");
									OnClientConnected?.Invoke(ID);
								}
								break;
							}
						case MessageType.ClientDisconnected: // Server said some client disconnected
							{
								var ID = components[1];
								clientIDs.Remove(ID);
								Console.Log($"Client '{ID}' disconnected. {ConnectedClients()}\n");
								OnClientDisconnected?.Invoke(ID);
								break;
							}
						case MessageType.ClientOnline: // Someone just connected and is getting updated on who is already online
							{
								var ID = components[1];
								if (ID == ClientUniqueID) // For me?
								{
									for (int i = 2; i < components.Length; i++)
									{
										var curClientID = components[i];

										if (clientIDs.Contains(curClientID) == false)
										{
											clientIDs.Add(curClientID);
										}
									}
									Console.Log($"{ConnectedClients()}\n");
								}
								break;
							}
						case MessageType.ClientToAll: // A client is sending a message to all clients
							{
								if (components[1] == ClientUniqueID) break; // Is this my message coming back to me?
								var msg = new Message(components[2], components[3], Receivers.AllClients, ClientUniqueID) { SenderClientUniqueID = components[1] };

								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case MessageType.ClientToAllAndServer: // A client is sending a message to the server and all clients
							{
								if (components[1] == ClientUniqueID) break; // Is this my message coming back to me?
								var msg = new Message(components[2], components[3], Receivers.ServerAndAllClients, ClientUniqueID)
								{ SenderClientUniqueID = components[1] };

								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case MessageType.ClientToClient: // A client is sending a message to another client
							{
								if (components[2] != ClientUniqueID) break; // Not for me?
								if (components[1] == ClientUniqueID) return; // Is this my message coming back to me? (unlikely)
								var msg = new Message(components[3], components[4], Receivers.Client, ClientUniqueID)
								{ SenderClientUniqueID = components[1] };

								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case MessageType.ServerToAll: // The server sent everyone a message
							{
								var msg = new Message(components[1], components[2], Receivers.AllClients, ClientUniqueID);

								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
						case MessageType.ServerToClient: // The server sent some client a message
							{
								if (components[1] != ClientUniqueID) return; // Not for me?

								var msg = new Message(components[1], components[2], Receivers.Client, ClientUniqueID);

								LogMessage(msg, false);
								OnMessageReceived?.Invoke(msg);
								break;
							}
					}
				}
				if (messageBack != "") client.SendAsync(messageBack);
			}
			protected override void OnError(SocketError error)
			{
				ClientIsConnected = false;
				Debug.LogError(-1, $"{error}", true);
			}
		}
	}
}
