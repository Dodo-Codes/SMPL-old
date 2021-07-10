using NetCoreServer;
using System.Net;
using System.Net.Sockets;
using TcpClient = NetCoreServer.TcpClient;

namespace SMPL
{
	public static class Multiplayer
	{
		private enum MessageType
		{
			Connection, ChangeID, ClientConnected, ClientDisconnected, ClientOnline, ClientToAll, ClientToClient, ClientToServer, ServerToAll, ServerToClient, ClientToAllAndServer
		}
		public enum MessageReceiver
		{
			Server, AllClients, ServerAndAllClients
		}

		internal static Server server;
		internal static Client client;

		public static string SameDeviceIP { get { return "127.0.0.1"; } }
		public static bool MessagesAreLogged { get; set; }

		public static void StartServer()
		{
			try
			{
				if (serverIsRunning)
				{
					Console.Log.Message.Do("Server is already starting/started.");
					return;
				}
				if (clientIsConnected)
				{
					Console.Log.Message.Do("Cannot start a server while a Client.");
					return;
				}
				server = new Simple.Server(IPAddress.Any, (int)serverPort);
				server.Start();
				serverIsRunning = true;

				var hostName = Dns.GetHostName();
				var hostEntry = Dns.GetHostEntry(hostName);
				connectToServerInfo = "Clients can connect through those IPs if they are in the same multiplayer\n" +
					"(device / router / Virtual Private Network programs like Hamachi or Radmin):\nSame device: 127.0.0.1";
				foreach (var ip in hostEntry.AddressList)
				{
					if (ip.AddressFamily == AddressFamily.InterNetwork)
					{
						var ipParts = ip.ToString().Split('.');
						var ipType = ipParts[0] == "192" && ipParts[1] == "168" ? "Same router: " : "Same VPN: ";
						connectToServerInfo = $"{connectToServerInfo}\n{ipType}{ip}";
					}
				}
				Console.Log.Message.Do($"Started a {Game.Title.Get()} LAN Server on port {serverPort}.\n\n" +
					$"{connectToServerInfo}");
				Console.Log.Message.Do("");
			}
			catch (Exception ex)
			{
				serverIsRunning = false;
				Console.Log.Message.Do($"Error: {ex.Message}");
			}
		}
		public static void StopServer()
		{
			try
			{
				if (serverIsRunning == false)
				{
					Console.Log.Message.Do("Server is not running.");
					return;
				}
				if (clientIsConnected)
				{
					Console.Log.Message.Do("Cannot stop a server while a client.");
					return;
				}
				serverIsRunning = false;
				server.Stop();
				Console.Log.Message.Do($"The LAN Server on port {serverPort} was stopped.");
			}
			catch (Exception ex)
			{
				serverIsRunning = false;
				Console.Log.Message.Do($"Error: {ex.Message}");
				return;
			}
		}

		public static void ConnectClient(string clientUniqueID, string serverIP)
      {
			if (clientIsConnected)
			{
				Console.Log.Message.Do("Already connecting/connected.");
				return;
			}
			if (serverIsRunning)
			{
				Console.Log.Message.Do("Cannot connect as Client while hosting a Server.");
				return;
			}

			try
			{
				client = new Simple.Client(data.ip, (int)serverPort);
			}
			catch (Exception)
			{
				Console.Log.Message.Do($"The IP '{data.ip}' is invalid.");
				return;
			}
			Console.Log.Message.Do($"Entering connection '{data.ip}:{serverPort}' as Client...");
			client.ConnectAsync();
		}
		public static void DisconnectClinet()
      {
			if (clientIsConnected == false)
			{
				Console.Log.Message.Do("Cannot disconnect when not connected as Client.");
				return;
			}
			client.DisconnectAndStop();
		}

		public static void SendMessage(MessageReceiver receiver, string message)
		{
			if (MessageDisconnected()) return;

			var log = "";
			switch (receiver)
			{
				case MessageReceiver.Server:
					{
						if (clientIsConnected)
						{
							client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToServer}{multiplayerMsgComponentSep}" +
								$"{data.clientID}{multiplayerMsgComponentSep}{message}");
							log = $"Message sent to Server: {message}";
						}
						break;
					}
				case MessageReceiver.AllClients:
					{
						if (clientIsConnected)
						{
							client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToAll}{multiplayerMsgComponentSep}" +
								$"{data.clientID}{multiplayerMsgComponentSep}{message}");
						}
						else if (serverIsRunning)
						{
							server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ServerToAll}{multiplayerMsgComponentSep}{message}");
						}
						log = $"Message sent to all Clients: {message}";
						break;
					}
				case MessageReceiver.ServerAndAllClients:
					{
						if (clientIsConnected)
						{
							client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToAllAndServer}{multiplayerMsgComponentSep}" +
								$"{data.clientID}{multiplayerMsgComponentSep}{message}");
						}
						else if (serverIsRunning)
						{
							server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ServerToAll}{multiplayerMsgComponentSep}{message}");
						}
						log = $"Message sent to Server & all Clients: {message}";
						break;
					}
			}
			Console.Log.Message.Do(log, log != "");
		}
		public static void SendPrivateMessage(string clientUniqueID, string message)
		{
			if (MessageDisconnected() || clientUniqueID == data.clientID) return;

			if (clientIsConnected)
			{
				client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.ClientToClient}{multiplayerMsgComponentSep}" +
					$"{clientUniqueID}{multiplayerMsgComponentSep}{clientUniqueID}{multiplayerMsgComponentSep}{message}");
			}
			else if (serverIsRunning)
			{
				server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ServerToClient}{multiplayerMsgComponentSep}" +
					$"{clientUniqueID}{multiplayerMsgComponentSep}{message}");
			}
			Console.Log.Message.Do($"Message sent to Client [{clientUniqueID}]: {message}");
		}

		private static bool MessageDisconnected()
		{
			if (clientIsConnected == false && serverIsRunning == false)
			{
				if (data.multiplayerLogMessagesToConsole == false) return true;
				Console.Log.Message.Do("Cannot send a message while disconnected.");
				return true;
			}
			return false;
		}
	}

	internal class Session : TcpSession
	{
		public Session(TcpServer server) : base(server) { }

		protected override void OnConnected()
		{
			// Send invite message
			//string message = "Hello from TCP! Please send a message!";
			//SendAsync(message);
		}
		protected override void OnDisconnected()
		{
			var disconnectedClient = clientRealIDs[Id.ToString()];
			clientIDs.Remove(disconnectedClient);
			server.Multicast($"{multiplayerMsgSep}{(int)MessageType.ClientDisconnected}{multiplayerMsgComponentSep}{disconnectedClient}");

			Console.Log.Message.Do($"Client [{disconnectedClient}] disconnected.");
			game.OnMultiplayerClientDisconnected(disconnectedClient);
		}
		protected override void OnReceived(byte[] buffer, long offset, long size)
		{
			var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
			var messages = rawMessages.Split(multiplayerMsgSep, StringSplitOptions.RemoveEmptyEntries);
			var messageBack = "";
			foreach (var message in messages)
			{
				var components = message.Split(multiplayerMsgComponentSep);
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
								messageBack = $"{multiplayerMsgSep}{(int)MessageType.ChangeID}{multiplayerMsgComponentSep}" +
									$"{id}{multiplayerMsgComponentSep}{clientID}";
							}
							clientRealIDs[Id.ToString()] = clientID;
							clientIDs.Add(clientID);

							// Sticking another message to update the newcoming client about online clients
							messageBack = $"{messageBack}{multiplayerMsgSep}{(int)MessageType.ClientOnline}{multiplayerMsgComponentSep}" +
								$"{clientID}";
							foreach (var ID in clientIDs)
							{
								messageBack = $"{messageBack}{multiplayerMsgComponentSep}{ID}";
							}

							// Sticking a third message to update online clients about the newcomer.
							messageBack =
								$"{messageBack}{multiplayerMsgSep}{(int)MessageType.ClientConnected}{multiplayerMsgComponentSep}" +
								$"{clientID}";
							Console.Log.Message.Do($"Client [{clientID}] connected.");
							game.OnMultiplayerClientConnected(clientID);
							break;
						}
					case MessageType.ClientToAll: // A client wants to send a message to everyone
						{
							messageBack = $"{messageBack}{multiplayerMsgSep}{message}";
							break;
						}
					case MessageType.ClientToClient: // A client wants to send a message to another client
						{
							messageBack = $"{messageBack}{multiplayerMsgSep}{message}";
							break;
						}
					case MessageType.ClientToServer: // A client sent me (the server) a message
						{
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(components[1], components[2]);
							break;
						}
					case MessageType.ClientToAllAndServer: // A client is sending me (the server) and all other clients a message
						{
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(components[1], components[2]);
							messageBack = $"{messageBack}{multiplayerMsgSep}{message}";
							break;
						}
				}
			}
			if (messageBack != "") server.Multicast(messageBack);
		}
		protected override void OnError(SocketError error)
		{
			Console.Log.Message.Do($"Error: {error}");
		}
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
			serverIsRunning = false;
			Console.Log.Message.Do($"Error: {error}");
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
			clientIsConnected = true;
			clientIDs.Add(data.clientID);
			Console.Log.Message.Do($"Connected as Client [{data.clientID}] to {client.Socket.RemoteEndPoint}.");
			game.OnMultiplayerConnected();
			client.SendAsync($"{multiplayerMsgSep}{(int)MessageType.Connection}{multiplayerMsgComponentSep}" +
				$"{client.Id}{multiplayerMsgComponentSep}{data.clientID}");
		}
		protected override void OnDisconnected()
		{
			if (clientIsConnected)
			{
				clientIsConnected = false;
				Console.Log.Message.Do("Disconnected.");
				clientIDs.Clear();
				game.OnMultiplayerDisconnected();
				if (stop == true) return;
			}

			// Wait for a while...
			Thread.Sleep(1000);

			// Try to connect again
			Console.Log.Message.Do("Lost connection. Trying to reconnect...");
			ConnectAsync();
		}
		protected override void OnReceived(byte[] buffer, long offset, long size)
		{
			var rawMessages = Encoding.UTF8.GetString(buffer, (int)offset, (int)size);
			var messages = rawMessages.Split(multiplayerMsgSep, StringSplitOptions.RemoveEmptyEntries);
			var messageBack = "";
			foreach (var message in messages)
			{
				var components = message.Split(multiplayerMsgComponentSep);
				var messageType = (MessageType)int.Parse(components[0]);
				switch (messageType)
				{
					case MessageType.ChangeID: // Server said someone's ID is taken and sent a free one
						{
							if (components[1] == client.Id.ToString()) // Is this for me?
							{
								var oldID = data.clientID;
								var newID = components[2];
								clientIDs.Remove(oldID);
								clientIDs.Add(newID);

								Console.Log.Message.Do($"Client ID [{oldID}] is taken. New Client ID is [{newID}].");
								game.OnMultiplayerTakenID(newID);
							}
							break;
						}
					case MessageType.ClientConnected: // Server said some client connected
						{
							var ID = components[1];
							if (ID != data.clientID) // If not me
							{
								clientIDs.Add(ID);
								Console.Log.Message.Do($"Client [{components[1]}] connected.");
								game.OnMultiplayerClientConnected(ID);
							}
							break;
						}
					case MessageType.ClientDisconnected: // Server said some client disconnected
						{
							var ID = components[1];
							clientIDs.Remove(ID);
							Console.Log.Message.Do($"Client [{components[1]}] disconnected.");
							game.OnMultiplayerClientDisconnected(ID);
							break;
						}
					case MessageType.ClientOnline: // Someone just connected and is getting updated on who is already online
						{
							var ID = components[1];
							if (ID == data.clientID) // For me?
							{
								for (int i = 2; i < components.Length; i++)
								{
									var curClientID = components[i];

									if (clientIDs.Contains(curClientID) == false)
									{
										clientIDs.Add(curClientID);
									}
								}
							}
							Console.Log.Message.Do("");
							break;
						}
					case MessageType.ClientToAll: // A client is sending a message to all clients
						{
							var ID = components[1];
							if (ID == data.clientID) break; // Is this my message coming back to me?
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(ID, components[2]);
							break;
						}
					case MessageType.ClientToAllAndServer: // A client is sending a message to the server and all clients
						{
							var ID = components[1];
							if (ID == data.clientID) break; // Is this my message coming back to me?
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[2]}");
							}
							game.OnMultiplayerMessageReceived(ID, components[2]);
							break;
						}
					case MessageType.ClientToClient: // A client is sending a message to another client
						{
							var ID = components[1];
							if (ID == data.clientID) return; // Is this my message coming back to me? (unlikely)
							if (components[2] != data.clientID) return; // Not for me?

							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Client [{components[1]}]: {components[3]}");
							}
							game.OnMultiplayerMessageReceived(ID, components[3]);
							break;
						}
					case MessageType.ServerToAll: // The server sent everyone a message
						{
							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Server: {components[1]}");
							}
							game.OnMultiplayerMessageReceived(null, components[1]);
							break;
						}
					case MessageType.ServerToClient: // The server sent some client a message
						{
							if (components[1] != data.clientID) return; // Not for me?

							if (data.multiplayerLogMessagesToConsole)
							{
								Console.Log.Message.Do($"Message received from Server: {components[1]}");
							}
							game.OnMultiplayerMessageReceived(null, components[1]);
							break;
						}
				}
			}
			if (messageBack != "") client.SendAsync(messageBack);
		}
		protected override void OnError(SocketError error)
		{
			clientIsConnected = false;
			Console.Log.Message.Do($"Error: {error}");
		}
	}
}
