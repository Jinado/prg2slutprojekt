using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public class Server
    {
        private TcpListener listener;
        private List<TcpClient> clients = new List<TcpClient>();
        private TcpClient client;
        private int port;
        private IPAddress ip;
        private int maxPlayers;
        private Form form;

        /// <summary>
        /// This constructor is used for constructing a <see cref="Server">Server</see> to host
        /// </summary>
        /// <param name="port"></param>
        /// <param name="maxPlayers"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidValueOfMaximumPlayersException"></exception>
        public Server(int maxPlayers, Form form, int port = 6767)
        {
            if (port < 1024)
                throw new ArgumentException("Invalid port number. The port cannot be within the range of 0 - 1023.");

            if (maxPlayers > 4 || maxPlayers < 2)
                throw new InvalidValueOfMaximumPlayersException();

            this.maxPlayers = maxPlayers;
            this.port = port;
            this.form = form;
        }

        /// <summary>
        /// This constructor is used for constructing a <see cref="Server">Server</see> to join
        /// </summary>
        /// <param name="ip">The IP-adress the server is running on</param>
        /// <param name="port">The port the server is running on</param>
        /// <exception cref="ArgumentException"></exception>
        public Server(TcpClient client, string ip, Form form, int port = 6767)
        {
            if (port < 1024)
                throw new ArgumentException("Invalid port number. The port cannot be within the range of 0 - 1023.");

            if (client == null)
                throw new ArgumentNullException();

            try
            {
                this.ip = IPAddress.Parse(ip);
            }
            catch(Exception err)
            {
                if(err.InnerException is ArgumentNullException)
                    MessageBox.Show("IP-adressen kan ej vara av typ null", "Ogiltig IP-adress", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (err.InnerException is FormatException)
                    MessageBox.Show("Den angivna IP-adressen är inkorrekt formaterad", "Ogiltig IP-adress", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(err.Message, "Ogiltig IP-adress", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            this.client = client;
            this.client.NoDelay = true;
            this.port = port;
            this.form = form;
        }

        /// <summary>
        /// Start a <see cref="Server">Server</see> to host a game on
        /// </summary>
        /// <param name="server">The <see cref="Server">Server</see> to host the game on</param>
        static public void StartServer(Server server)
        {
            try
            {
                server.listener = new TcpListener(IPAddress.Any, server.port);
                server.listener.Start();
            }
            catch (Exception err) { MessageBox.Show(err.Message, "Fel vid start av server", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            ListenForConnections(server);
        }

        /// <summary>
        /// Listens for connections from <see cref="TcpClient">Clients</see>
        /// </summary>
        /// <param name="server">The server to listen on</param>
        private async static void ListenForConnections(Server server)
        {
            TcpClient tempClient = null;

            try
            {
                tempClient = await server.listener.AcceptTcpClientAsync();
                if (tempClient != null)
                    server.clients.Add(tempClient);
            }
            catch (Exception err)
            {
                // This is true when the server has been stopped. 
                // The return statement makes sure we do not continue
                // to listen for further connections.
                if (err is ObjectDisposedException) return;
                MessageBox.Show(err.Message, "Anslutningsfel 1", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (tempClient != null)
                RecieveData(server, tempClient);
            ListenForConnections(server);
        }

        /// <summary>
        /// Join a <see cref="Server">Server</see>
        /// </summary>
        /// <param name="server">The <see cref="Server">Server</see> to join</param>
        /// <returns>True if the connections was successful</returns>
        public async static Task<bool> JoinServer(Server server)
        {
            try
            {
                await server.client.ConnectAsync(server.ip, server.port);

                if (server.client.Connected)
                {
                    // Start listening for messages
                    RecieveDataFromServer(server);
                    return true;
                }
            }
            catch (Exception err) { MessageBox.Show(err.Message, "Anslutningsfel 2", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }

            return false;
        }

        /// <summary>
        /// Disconnect a <see cref="Player">Player</see> from a <see cref="Server">Server</see>
        /// </summary>
        /// <param name="player">The <see cref="Player">Player</see> to disconnect</param>
        /// <param name="server">The <see cref="Server">Server</see> to disconnect from</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void Disconnect(Player player, Server server, bool host)
        {
            Server.PlayerDisconnected(server, player, host);
        }

        /// <summary>
        /// Disconnect a specific client
        /// </summary>
        /// <param name="server">The server to disconnect from</param>
        public static void Disconnect(Server server)
        {
            // NAD stands for "NAME AVAILABLE DISCONNECT", this is used
            // to identify what type of message has been recieved/sent
            SendMessageToServer(server, "NAD");
        }

        /// <summary>
        /// Stop the <see cref="Server">Server</see>
        /// </summary>
        /// <param name="server">The <see cref="Server">Server</see> to stop</param>
        public static void Stop(Server server)
        {
            Server.ForceDisconnectPlayers(server);
            server.listener.Server.Close();
        }

        /// <summary>
        /// Announce to each client that a <see cref="Player">Player</see> has disconnected
        /// </summary>
        /// <param name="server">The <see cref="Server">Server</see> the <see cref="Player">Player</see> has disconnected from</param>
        /// <param name="player">The <see cref="Player">Player</see> that has disconnected</param>
        /// <param name="host">True if the sender of the message is the host</param>
        private static void PlayerDisconnected(Server server, Player player, bool host)
        {
            // PLD stands for "PLAYER DISCONNECTED", this is used
            // to identify what type of message has been recieved/sent
            string message = $"PLD|{player.Name}";
            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);           
        }

        /// <summary>
        /// Send a message to each client from the host that a player has been disconnected
        /// </summary>
        /// <param name="server">The server to disconnect from</param>
        private static void ForceDisconnectPlayers(Server server)
        {
            // FDP stands for "FORCE DISCONNECT PLAYER", this is used
            // to identify what type of message has been recieved/sent
            string message = "FDP|";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Check with the server if a name is available
        /// </summary>
        /// <param name="server">The server to ask</param>
        /// <param name="name">The name you wish to use</param>
        public static void IsNameAvailable(Server server, string name)
        {
            // INA stands for "IS NAME AVAILABLE", this is used
            // to identify what type of message has been recieved/sent
            string message = $"INA|{name}";
            SendMessageToServer(server, message);
        }

        public static void NameAvailableResult(Server server, string result)
        {
            // NAR stands for "NAME AVAILABLE RESULT", this is used
            // to identify what type of message has been recieved/sent
            string message = $"NAR|{result}";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Inform all other <see cref="TcpClient">Clients</see> on the server of a move
        /// </summary>
        /// <param name="server">The <see cref="Server">Server</see> to inform</param>
        /// <param name="square">The <see cref="Square">Square</see> to move a <see cref="Character">Character</see> to</param>
        /// <param name="character">The <see cref="Character">Character</see> to move</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void MoveCharacter(Server server, Square square, Character character, bool host)
        {
            // MVC stands for "MOVE CHARACTER", this is used
            // to identify what type of message has been recieved/sent
            string message = $"MVC|{square.Position}|{character.Position}";
            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Inform all other clients which <see cref="Player">Player</see> has won
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="player">The <see cref="Player">Player</see> that has won the game</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void HasWon(Server server, Player player, bool host)
        {
            // HAW stands for "HAS WON", this is used
            // to identify what type of message has been recieved/sent
            string message = $"HAW|{player.Name}";
            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Tell each client that the dice has been thrown and show the result
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="diceResult">The result of the dice throw</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void ThrownDice(Server server, byte diceResult, bool host)
        {
            // TRD stands for "THROWN DICE", this is used 
            // to identify what type of message has been recieved/sent
            string message = $"TRD|{diceResult}";
            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Tell the other clients which <see cref="Player">Player's</see> turn it is
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="player">The <see cref="Player">Player</see> whose turn it is</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void ChangeTurn(Server server, Player player, bool host)
        {
            // CHT stands for "CHANGE TURN", this is used
            // to identify what type of message has been recieved/sent
            string message = $"CHT|{player.Name}";
            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Send over a client's <see cref="Player">Player</see> object to all other clients
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="player">The <see cref="Player">Player</see> object to send over</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void SendPlayerData(Server server, Player player, bool host)
        {
            string message = null;
            if(player.Characters == null)
                // SPN stands for "SEND PLAYER DATA (NO CHARACTERS)", this is used
                // to identify what type of message has been recieved/sent
                message = $"SPN|{player.Name}|{(int)player.State}";
            else
                // SPD stands for "SEND PLAYER DATA", this is used
                // to identify what type of message has been recieved/sent
                message = $"SPD|{player.Name}|{player.Characters[0].Colour}|{(int)player.State}";

            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Informs all clients of maximum amount of players
        /// </summary>
        /// <param name="server">The server to broadcast the message on</param>
        /// <param name="maxPlayers">The max amount of players</param>
        public static void SendMaxPlayers(Server server, int maxPlayers)
        {
            // SMP stands for "SEND MAX PLAYERS", this is used
            // to identify what type of message has been recieved/sent
            string message = $"SMP|{maxPlayers}";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Send over a client's Player object to all other clients, also inform the client
        /// of the total number of Player objects that will be sent
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="player">The player object to send over</param>
        /// <param name="count">This keeps track of how many Player objects will be sent over in total</param>
        public static void SendPlayerData(Server server, Player player, int count)
        {
            /*
             * If I do not run the program with the below Thread.Sleep() call, the client crashes.
             * The reason is because the code runs too fast without it. If I do not sleep, the below
             * message variable somehow mutates and concatenates two messages into one and sends 
             * that message to the client. When the client recieves this message it does not know
             * how to read it and ends up crashing. If you were to remove the Thread.Sleep() call
             * in this method, and then run the program in debug mode and step through the code 
             * slowly, you will see no errors because the code is running slowly. If you were to
             * remove the below line of code and then NOT step through the code, but run it 
             * normally, the above explained bug will happen when a THIRD player tries to 
             * connect to the server.
             * 
             * Below is an example of the type of message the variable "message" should contain:
             * "SPD|Jinado|(Color [Yellow])|1|3"
             * 
             * But instead it ends up looking something like this when the bug occurs:
             * "SPD|Jinado|(Color [Yellow])|1|3SPD|Flax|(Color [Red])|0|3"
            */
            Thread.Sleep(50);

            // SPD stands for "SEND PLAYER DATA", this is used
            // to identify what type of message has been recieved/sent
            string message = $"SPD|{player.Name}|{player.Characters[0].Colour}|{(int)player.State}|{count}";

            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Tell all other clients which players are ready and which aren't
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="players">The list of players connected to the server</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void SendReadyStatus(Server server, List<Player> players, bool host)
        {
            // SRS stands for "SEND READY STATUS", this is used 
            // to identify what type of message has been recieved/sent
            string message = "SRS";
            foreach(Player p in players)
                message += $"|{p.Name}|{(int)p.State}";

            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Recieve data sent to the server and broadcast it to all clients
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="client">The client that will listen to messages (I.E. The server's client itself)</param>
        private async static void RecieveData(Server server, TcpClient client)
        {
            // Client is null if the server has been stopped, in that case, don't continue
            // execution of this method
            if (client != null)
            {
                // If the client isn't null, check if the client has disconnected. If it has, client.Client is equal to NULL
                if (client.Client == null || !client.Connected)
                {
                    server.clients.Remove(client);
                    return;
                }
            }
            else
            {
                return;
            }

            byte[] buffer = new byte[1024];

            int n = 0;
            try
            {
                n = await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            }
            catch (Exception err)
            {
                // One of these errors get caught when a client has disconnected
                if(err is InvalidOperationException)
                {
                    server.clients.Remove(client);
                    return;
                }
                else if(err.InnerException is SocketException)
                {
                    server.clients.Remove(client);
                    return;
                }
                MessageBox.Show(err.Message, "Serverfel 1", MessageBoxButtons.OK, MessageBoxIcon.Error); return; 
            }

            string message = Encoding.UTF8.GetString(buffer, 0, n);
            if (message != null && message != "")
            {
                // Broadcast the message
                foreach (TcpClient clt in server.clients)
                {
                    // Make sure NOT to send the message back to origin UNLESS it's the user's ready status
                    if (!clt.Equals(client) || $"{message[0]}{message[1]}{message[2]}".Equals("SRS"))
                    {
                        SendMessage(clt, message);
                    }
                }

                // Send the message to oneself
                string msgType = $"{message[0]}{message[1]}{message[2]}";
                switch (msgType)
                {
                    case "PLD": // Player disconnected
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        else
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "FDP": // Player forcefully disconnected
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        else
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "MVC": // A character has been moved
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "HAW": // A player has won
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "TRD": // The dice have been thrown
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "CHT": // The turn has been changed
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "SMP": // Send an number informing the clients of the max amount of players
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "INA": // A client wishes to know if a name is available
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "NAR": // A result from the above request has been recieved
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SPD": // Player data has been sent
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SPN":
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SRS": // Ready status of all players have been sent
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                server.clients.Remove(client);
                client.Client.Dispose();
                client.Dispose();
                return;
            }

            RecieveData(server, client);
        }

        /// <summary>
        /// Listens for messages sent from the server to the client.
        /// </summary>
        private async static void RecieveDataFromServer(Server server)
        {
            byte[] buffer = new byte[1024];

            int n = 0;
            try
            {
                if (server.client != null) // Is null when the server has been stopped
                    if (server.client.Connected)
                        n = await server.client.GetStream().ReadAsync(buffer, 0, buffer.Length);
                    else return;
                else return;
            }
            catch (Exception err)
            {
                // This is true when the client has disconnnected. 
                // The return statement makes sure we do not continue
                // to listen for more data.
                if (err is ObjectDisposedException || err.InnerException is ObjectDisposedException) return;

                // This is true if the host stops the server and exists the application before the other clients exit
                if (err.InnerException is SocketException) return;
                MessageBox.Show(err.Message, "Serverfel 2", MessageBoxButtons.OK, MessageBoxIcon.Error); return; 
            }

            string message = Encoding.UTF8.GetString(buffer, 0, n);

            if (message != null)
            {
                string msgType = $"{message[0]}{message[1]}{message[2]}";
                switch (msgType)
                {
                    case "PLD": // Player disconnected
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        else
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "FDP": // Player forcefully disconnected
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        else
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "MVC": // A character has been moved
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "HAW": // A player has won
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "TRD": // The dice have been thrown
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "CHT": // The turn has been changed
                        (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "SMP": // Send an number informing the clients of the max amount of players
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "INA": // A client wishes to know if a name is available
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "NAR": // A result from the above request has been recieved
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SPD": // Player data has been sent
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SPN":
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SRS": // Ready status of all players have been sent
                        (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    default:
                        break;
                }
            }

            RecieveDataFromServer(server);
        }

        /// <summary>
        /// Send the message to the server
        /// </summary>
        /// <param name="server">The server to send the message to</param>
        /// <param name="message">The message to send</param>
        private async static void SendMessageToServer(Server server, string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);

            try
            {
                    await server.client.GetStream().WriteAsync(msg, 0, msg.Length);
            }
            catch (Exception err) { MessageBox.Show($"Kunde ej skicka meddelande till servern.\n{err.Message}", "Serverfel 3", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            // If the message is that a player has disconnected, close the client
            // linked to that player
            if ($"{message[0]}{message[1]}{message[2]}".Equals("PLD") || $"{message[0]}{message[1]}{message[2]}".Equals("NAD"))
            {
                server.client.Client.Dispose();
            }
        }

        /// <summary>
        /// Sends a message generated on the host directly to the other connected clients
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="message">The message to send</param>
        private static void SendMessageFromHost(Server server, string message)
        {
            if (message != null)
            {
                // Broadcast the message
                foreach (TcpClient clt in server.clients)
                {
                    SendMessage(clt, message);
                }
            }
        }


        /// <summary>
        /// Send a message to another client
        /// </summary>
        /// <param name="client">The client to send the message to</param>
        /// <param name="message">The message to send</param>
        private async static void SendMessage(TcpClient client, string message)
        {
            byte[] msg = Encoding.UTF8.GetBytes(message);

            try
            {
                if (client != null)
                    await client.GetStream().WriteAsync(msg, 0, msg.Length);
                else return;
            }
            catch (Exception err) { MessageBox.Show($"Kunde ej skicka meddelande till klienten.\n{err.Message}", "Serverfel 4", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        }

        public TcpClient Client
        {
            private set { this.client = value; }
            get
            {
                return this.client;
            }
        }

        public override string ToString()
        {
            if (this.ip != null)
                return $"{this.ip}:{this.port}";
            else
                return $"Port: {this.port}";
        }
    }
}
