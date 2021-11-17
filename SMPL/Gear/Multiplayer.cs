using NetCoreServer;
using NetFwTypeLib;
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
		private static readonly Dictionary<Guid, string> clientRealIDs = new();
		private static readonly List<string> clientIDs = new();
		private static readonly int serverPort = 1234;
		private static Guid id;
		private static Udp udp;

		private static string MessageToString(Message message)
		{
			var unr = message.Unreliable ? "0" : "1";
			return
				$"{Message.SEP}" +
				$"{(int)message.type}{Message.COMP_SEP}" +
				$"{message.SenderUniqueID}{Message.COMP_SEP}" +
				$"{message.ReceiverUniqueID}{Message.COMP_SEP}" +
				$"{(int)message.Receivers}{Message.COMP_SEP}" +
				$"{message.Tag}{Message.COMP_SEP}" +
				$"{message.Content}{Message.COMP_SEP}" +
				$"{unr}";
		}
		private static List<Message> StringToMessages(string message)
		{
			var result = new List<Message>();
			var split = message.Split(Message.SEP, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < split.Length; i++)
			{
				if (split[i].Length < 10) continue;
				var comps = split[i].Split(Message.COMP_SEP);
				result.Add(new Message()
				{
					type = (Message.Type)int.Parse(comps[0]),
					SenderUniqueID = comps[1],
					ReceiverUniqueID = comps[2],
					Receivers = (Message.Toward)int.Parse(comps[3]),
					Tag = comps[4],
					Content = comps[5],
					Unreliable = comps[6] == "1"
				});
			}
			return result;
		}
		private static bool MessageDisconnected()
		{
			if (ClientIsConnected == false && ServerIsRunning == false)
			{
				Debug.LogError(2, "Cannot send a message while disconnected.", true);
				return true;
			}
			return false;
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
										Message.Toward.Client, null, msg.Content, receiverClientUniqueID: msg.SenderUniqueID)
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
								var onlineMsg = new Message(Message.Toward.Client, null, null, receiverClientUniqueID: msg.SenderUniqueID)
								{ type = Message.Type.ClientOnline };
								for (int j = 0; j < clientIDs.Count; j++)
								{
									if (onlineMsg.Content == null)
									{
										onlineMsg.Content = clientIDs[j];
										continue;
									}
									onlineMsg.Content += $"{Message.TEMP_SEP}{clientIDs[j]}";
								}
								messageBack += MessageToString(onlineMsg);

								// Sticking a third message to update online clients about the newcomer.
								var newComMsg = new Message(Message.Toward.AllClients, null, msg.SenderUniqueID)
								{ type = Message.Type.ClientConnected };
								messageBack += MessageToString(newComMsg);
								Console.Log($"Client '{msg.SenderUniqueID}' connected. {ConnectedClients()}\n");
								Events.Notify(Events.Type.ClientConnect, new() { String = new string[] { msg.SenderUniqueID } });
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
								Events.Notify(Events.Type.MessageReceived, new() { Message = msg });
								break;
							}
						case Message.Type.ClientToAllAndServer: // A client is sending me (the server) and all other clients a message
							{
								Events.Notify(Events.Type.MessageReceived, new() { Message = msg });
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
									Events.Notify(Events.Type.ClientTakenUniqueID, new() { String = new string[] { oldID } });
								}
								break;
							}
						case Message.Type.ClientConnected: // Server said some client connected
							{
								if (msg.Content != ClientUniqueID) // If not me
								{
									clientIDs.Add(msg.Content);
									Console.Log($"Client '{msg.Content}' connected. {ConnectedClients()}\n");
									Events.Notify(Events.Type.ClientConnect, new() { String = new string[] { msg.Content } });
								}
								// when it's me it's handled in Client.OnConnected overriden method
								break;
							}
						case Message.Type.ClientDisconnected: // Server said some client disconnected
							{
								clientIDs.Remove(msg.Content);
								Console.Log($"Client '{msg.Content}' disconnected. {ConnectedClients()}\n");
								Events.Notify(Events.Type.ClientDisconnect, new() { String = new string[] { msg.Content } });
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
								Events.Notify(Events.Type.MessageReceived, new() { Message = msg });
								break;
							}
						case Message.Type.ClientToAllAndServer: // A client is sending a message to the server and all clients
							{
								if (msg.SenderUniqueID == ClientUniqueID) break; // Is this my message coming back to me?
								Events.Notify(Events.Type.MessageReceived, new() { Message = msg });
								break;
							}
						case Message.Type.ClientToClient: // A client is sending a message to another client
							{
								if (msg.ReceiverUniqueID != ClientUniqueID) break; // Not for me? Not interested.
								if (msg.SenderUniqueID == ClientUniqueID) return; // Is this my message coming back to me? (unlikely)
								Events.Notify(Events.Type.MessageReceived, new() { Message = msg });
								break;
							}
						case Message.Type.ServerToAll: // The server sent everyone a message
							{
								Events.Notify(Events.Type.MessageReceived, new() { Message = msg });
								break;
							}
						case Message.Type.ServerToClient: // The server sent some client a message
							{
								if (msg.ReceiverUniqueID != ClientUniqueID) return; // Not for me?
								Events.Notify(Events.Type.MessageReceived, new() { Message = msg });
								break;
							}
					}
				}
			}
		}
		private static void OpenPort()
		{
			var tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
			var fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
			var currentProfiles = fwPolicy2.CurrentProfileTypes;

			var inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
			inboundRule.Enabled = true;
			inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;
			inboundRule.Protocol = 6; // TCP
			inboundRule.Name = $"SMPL Multiplayer";
			inboundRule.Profiles = currentProfiles;

			var firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
			firewallPolicy.Rules.Add(inboundRule);
		}

		//=================

		internal class Udp
		{
			private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			private readonly Socket _sendSocket = new(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			private const int bufSize = 8 * 1024;
			private readonly State state = new();
			private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
			private EndPoint epTo;
			private AsyncCallback recv = null;
			private AsyncCallback send = null;

			public class State
			{
				public byte[] buffer = new byte[bufSize];
			}

			public void Server()
			{
				try
				{
					_socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
					_socket.Bind(new IPEndPoint(IPAddress.Any, serverPort));
					InitSocket();
				}
				catch (Exception ex)
				{
					Console.Log($"ERROR: {ex.Message}");
				}
			}
			public void Client(IPAddress ip)
			{
				try
				{
					_socket.Connect(ip, serverPort);
					InitSocket();
				}
				catch (Exception ex)
				{
					Console.Log($"ERROR: {ex.Message}");
				}
			}
			public void Send(string text)
			{
				try
				{
					var data = Encoding.ASCII.GetBytes(text);
					_socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
					{
						var so = (State)ar.AsyncState;
						int bytes = _socket.EndSend(ar);
					}, state);
				}
				catch (Exception ex)
				{
					Console.Log($"ERROR: {ex.Message}");
				}
			}
			private void InitSocket()
			{
				_socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
				{
					var so = (State)ar.AsyncState;
					int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
					_socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);
					DecodeMessages(id, Encoding.ASCII.GetString(so.buffer, 0, bytes));
				}, state);
			}
			private void InitSend(IPAddress ip)
			{
				var epTo = new IPEndPoint(ip, serverPort);
				_socket.BeginSendTo(state.buffer, 0, bufSize, SocketFlags.None, epTo, send = (ar) =>
				{
					var so = (State)ar.AsyncState;
					int bytes = _socket.EndSendTo(ar);
					_socket.BeginSendTo(so.buffer, 0, bufSize, SocketFlags.None, epTo, send, so);
				}, state);
			}

			public void Disconnect()
			{
				_socket.Close();
			}
		}
		internal class Session : TcpSession
		{
			public Session(TcpServer server) : base(server) { }

			protected override void OnConnected() { id = Id; }
			protected override void OnDisconnected()
			{
				var disconnectedClient = clientRealIDs[Id];
				clientRealIDs.Remove(Id);
				clientIDs.Remove(disconnectedClient);
				var msg = new Message(Message.Toward.AllClients, null, disconnectedClient)
				{ type = Message.Type.ClientDisconnected };
				SendMessage(msg);

				Console.Log($"Client '{disconnectedClient}' disconnected. {ConnectedClients()}\n");
				Events.Notify(Events.Type.ClientDisconnect, new() { String = new string[] { disconnectedClient } });
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
				Events.Notify(Events.Type.ServerStop);
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
				id = Id;
				ClientIsConnected = true;
				clientIDs.Add(ClientUniqueID);
				var ip = client.Socket.RemoteEndPoint.ToString().Split(':')[0];
				if (ServerIsRunning == false) Console.Log($"Connected as '{ClientUniqueID}' to {Window.Title} LAN Server[{ip}].\n");

				Events.Notify(Events.Type.ClientConnect, new() { String = new string[] { ClientUniqueID } });

				var msg = new Message(Message.Toward.Server, null, Id.ToString()) { type = Message.Type.Connection };
				client.SendAsync(MessageToString(msg));
			}
			protected override void OnDisconnected()
			{
				if (ClientIsConnected)
				{
					ClientIsConnected = false;
					Console.Log("Disconnected from the LAN Server.\n");
					clientIDs.Clear();
					Events.Notify(Events.Type.ClientDisconnect, new() { String = new string[] { ClientUniqueID } });
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
			internal const string SEP = ";$*#", COMP_SEP = "=!@,", TEMP_SEP = ")`.&";

			public string Content { get; set; }
			public string Tag { get; set; }
			public string ReceiverUniqueID { get; set; }
			public string SenderUniqueID { get; internal set; }
			public Toward Receivers { get; set; }
			public bool Unreliable { get; set; }
			internal Type type;

			public Message(Toward receivers, string tag, string content, bool unreliable = false, string receiverClientUniqueID = null)
			{
				Content = content;
				Tag = tag;
				ReceiverUniqueID = receiverClientUniqueID;
				SenderUniqueID = ClientUniqueID;
				Unreliable = unreliable;
				Receivers = receivers;
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
				var reliable = Unreliable ? "Unreliable" : "Reliable";
				var rec = Receivers == Toward.Client ?
					$"to Client '{ReceiverUniqueID}'" : $"to {Receivers}";
				return
					$"{reliable} Multiplayer Message {send} {rec}\n" +
					$"Tag: {Tag}\n" +
					$"Content: {Content}";
			}
		}

		public static class Event
		{
			public static class Subscribe
			{
				public static void ServerStart(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.ServerStart, thingUID, order);
				public static void ServerStop(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.ServerStop, thingUID, order);
				public static void ClientConnect(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.ClientConnect, thingUID, order);
				public static void ClientDisconnect(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.ClientDisconnect, thingUID, order);
				public static void ClientTakenUniqueID(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.ClientTakenUniqueID, thingUID, order);
				public static void MessageReceive(string thingUID, uint order = uint.MaxValue) =>
					Events.Enable(Events.Type.MessageReceived, thingUID, order);
			}
			public static class Unsubscribe
			{
				public static void ServerStart(string thingUID) =>
					Events.Disable(Events.Type.ServerStart, thingUID);
				public static void ServerStop(string thingUID) =>
					Events.Disable(Events.Type.ServerStop, thingUID);
				public static void ClientConnect(string thingUID) =>
					Events.Disable(Events.Type.ClientConnect, thingUID);
				public static void ClientDisconnect(string thingUID) =>
					Events.Disable(Events.Type.ClientDisconnect, thingUID);
				public static void ClientTakenUniqueID(string thingUID) =>
					Events.Disable(Events.Type.ClientTakenUniqueID, thingUID);
				public static void MessageReceive(string thingUID) =>
					Events.Disable(Events.Type.MessageReceived, thingUID);
			}
		}

		public const string SameDeviceIP = "127.0.0.1";
		public static bool ClientIsConnected { get; private set; }
		public static bool ServerIsRunning { get; private set; }
		public static string ClientUniqueID { get; private set; }

		public static void StartServer()
		{
			try
			{
				if (ServerIsRunning) { Debug.LogError(1, "Server is already starting/started.", true); return; }
				if (ClientIsConnected) { Debug.LogError(1, "Cannot start a Server while a Client.", true); return; }

				OpenPort();

				udp = new();
				udp.Server();
				server = new Server(IPAddress.Any, serverPort);
				server.Start();
				ServerIsRunning = true;

				Console.Log($"Started a {Window.Title} LAN Server.\n{GetServerConnectInfo()}\n");
				Events.Notify(Events.Type.ServerStop);
			}
			catch (Exception ex)
			{
				ServerIsRunning = false;
				if (ex.Message.Contains("Access is denied"))
					Debug.LogError(1, $"In order to start the {Window.Title} Multiplayer Server, run the game as an Administrator.", true);
				else
					Debug.LogError(1, ex.Message, true);
				Events.Notify(Events.Type.ServerStop);
			}
		}
		public static void StopServer()
		{
			try
			{
				if (ServerIsRunning == false) { Debug.LogError(1, "Server is not running.\n", true); return; }
				if (ClientIsConnected) { Debug.LogError(1, "Cannot stop a server while a client.\n", true); return; }
				ServerIsRunning = false;
				udp.Disconnect();
				server.Stop();
				Console.Log($"The {Window.Title} LAN Server was stopped.\n");
				Events.Notify(Events.Type.ServerStop);
			}
			catch (Exception ex)
			{
				ServerIsRunning = false;
				Debug.LogError(-1, ex.Message, true);
				Events.Notify(Events.Type.ServerStop);
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
				udp = new();
				udp.Client(IPAddress.Parse(serverIP));
			}
			catch (Exception)
			{
				Debug.LogError(1, $"The IP '{serverIP}' is invalid.\n", true);
				return;
			}

			client.ConnectAsync();
			ClientUniqueID = clientUniqueID;
			ClientIsConnected = true;
			Console.Log($"Connecting to {Window.Title} LAN Server[{serverIP}]...\n");
		}
		public static void DisconnectClinet()
		{
			if (ClientIsConnected == false)
			{
				Debug.LogError(1, "Cannot disconnect when not connected as Client.\n", true);
				return;
			}
			udp.Disconnect();
			client.DisconnectAndStop();
		}

		public static void SendMessage(Message message)
		{
			if (MessageDisconnected()) return;
			if (ServerIsRunning && message.Receivers == Message.Toward.Server) return;

			var msgStr = MessageToString(message);
			if (message.Unreliable)
				udp.Send(msgStr);
			else
			{
				if (ClientIsConnected) client.SendAsync(msgStr);
				else server.Multicast(msgStr);
			}
		}
		private static string GetServerConnectInfo()
		{
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
				var isRouter = ipParts[0] == "192" && ipParts[1] == "168";
				var ipType = isRouter ? "Same router: " : "Same VPN: ";
				connectToServerInfo = $"{connectToServerInfo}\n{ipType}{hostEntry.AddressList[i]}";
			}
			return connectToServerInfo;
		}
	}
}
