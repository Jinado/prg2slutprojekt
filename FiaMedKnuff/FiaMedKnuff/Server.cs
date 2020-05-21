using System;
using System.Collections.Generic;
using System.Drawing;
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
        /// This constructor is used for constructing a <see cref="Server"/> to host
        /// </summary>
        /// <param name="maxPlayers">The max amount of players that may be connected to the server at once</param>
        /// <param name="form">The form that the server should send the messages to</param>
        /// <param name="port">The port to use for the server</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="InvalidValueOfMaximumPlayersException"/>
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
        /// This constructor is used for constructing a <see cref="Server"/> to join
        /// </summary>
        /// <param name="ip">The IP-adress the server is running on</param>
        /// <param name="port">The port the server is running on</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="ArgumentNullException"/>
        public Server(TcpClient client, string ip, Form form, int port = 6767)
        {
            if (port < 1024)
                throw new ArgumentException("Invalid port number. The port cannot be within the range of 0 - 1023.");
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

            this.client = client ?? throw new ArgumentNullException();
            this.client.NoDelay = true;
            this.port = port;
            this.form = form;
        }

        /// <summary>
        /// Start a <see cref="Server"/> to host a game on
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to host the game on</param>
        /// <exception cref="SocketException"/>
        static public void StartServer(Server server)
        {
            try
            {
                server.listener = new TcpListener(IPAddress.Any, server.port);
                server.listener.Start();
            }
            catch (Exception err) 
            { 
                if(err is SocketException) throw err;
                MessageBox.Show(err.Message, "Fel vid start av server", MessageBoxButtons.OK, MessageBoxIcon.Error); return; 
            }

            ListenForConnections(server);
        }

        /// <summary>
        /// Listens for connections from <see cref="TcpClient">clients</see>
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to listen on</param>
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
        /// Join a <see cref="Server"/>
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to join</param>
        /// <returns>True if the connection was successful</returns>
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
        /// Disconnect a <see cref="Player"/> from a <see cref="Server"/>
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to disconnect from</param>
        /// <param name="player">The <see cref="Player"/> to disconnect</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void Disconnect(Server server, Player player, bool host)
        {
            PlayerDisconnected(server, player, host);
        }

        /// <summary>
        /// Disconnect a specific client
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to disconnect from</param>
        public static void Disconnect(Server server)
        {
            // NAD stands for "NAME AVAILABLE DISCONNECT", this is used
            // to identify what type of message has been recieved/sent
            SendMessageToServer(server, "NAD");
        }

        /// <summary>
        /// Stop the <see cref="Server"/>
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to stop</param>
        public static void Stop(Server server)
        {
            ForceDisconnectPlayers(server);
            server.listener.Server.Close();
        }

        /// <summary>
        /// This method is called by the host if he stops the <see cref="Server"/> mid-game
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to stop</param>
        public static void StopDuringGame(Server server)
        {
            // SDG stands for "STOP DURING GAME", this is used
            // to identify what type of message has been recieved/sent
            string message = "SDG";
            SendMessageFromHost(server, message);
            server.listener.Server.Close();
        }

        /// <summary>
        /// This method is called by the host if he leaves mid-game
        /// </summary>
        /// <param name="server">The <see cref="Server"/> he's hosting</param>
        public static void LeaveToLobby(Server server)
        {
            // LTL stands for "LEAVE TO LOBBY", this is used
            // to identify what type of message has been recieved/sent
            string message = "LTL";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Announce to each client that a <see cref="Player"/> has disconnected
        /// </summary>
        /// <param name="server">The <see cref="Server"/> the <see cref="Player"/> has disconnected from</param>
        /// <param name="player">The <see cref="Player"/> that has disconnected</param>
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
        /// Send a message to each client from the host that a <see cref="Player"/> has been disconnected
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to disconnect from</param>
        private static void ForceDisconnectPlayers(Server server)
        {
            // FDP stands for "FORCE DISCONNECT PLAYER", this is used
            // to identify what type of message has been recieved/sent
            string message = "FDP";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Ask the <see cref="Server"/> if it is full
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to ask</param>
        /// <param name="name">The name you wish to use</param>
        public static void IsServerFull(Server server, string name)
        {
            // ISF stands for "IS SERVER FULL", this is used
            // to identify what type of message has been recieved/sent
            string message = $"ISF|{name}";
            SendMessageToServer(server, message);
        }

        /// <summary>
        /// Send the result of the "Is the server full?" request back to the client
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="result">The result of the query</param>
        public static void ServerFullResult(Server server, string result)
        {
            // SFR stands for "SERVER FULL RESULT", this is used
            // to identify what type of message has been recieved/sent
            string message = $"SFR|{result}";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Check with the <see cref="Server"/> if a name is available
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to ask</param>
        /// <param name="name">The name you wish to use</param>
        public static void IsNameAvailable(Server server, string name)
        {
            // INA stands for "IS NAME AVAILABLE", this is used
            // to identify what type of message has been recieved/sent
            string message = $"INA|{name}";
            SendMessageToServer(server, message);
        }

        /// <summary>
        /// Send the result of the "Is this name available?" request back to the client
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="result">The result of the query</param>
        public static void NameAvailableResult(Server server, string result)
        {
            // NAR stands for "NAME AVAILABLE RESULT", this is used
            // to identify what type of message has been recieved/sent
            string message = $"NAR|{result}";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Inform all other clients on the server of a move
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="square">The <see cref="Square"/> to move to</param>
        /// <param name="character">The <see cref="Character"/> to move</param>
        /// <param name="pathLocation">The <see cref="Point">location</see> of the squares's <see cref="PictureBox"/></param>
        /// <param name="pbxCharacter">The character's <see cref="PictureBox"/></param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void MoveCharacter(Server server, Square square, Character character, Point pathLocation, PictureBox pbxCharacter, bool host)
        {
            // MVC stands for "MOVE CHARACTER", this is used
            // to identify what type of message has been recieved/sent
            string message = $"MVC|{square.Position}|{character.Position}|{character.Colour}|{pathLocation.X}|{pathLocation.Y}|{pbxCharacter.Name}";
            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Inform all other clients which <see cref="Player"/> has won
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="playerColour">The <see cref="Color"/> of the <see cref="Player"/> that won</param>
        /// <param name="host">True if the sender of the message is the host</param>s
        public static void HasWon(Server server, Color playerColour, bool host)
        {
            // HAW stands for "HAS WON", this is used
            // to identify what type of message has been recieved/sent
            string message = $"HAW|{Character.ColourToString(playerColour)}";
            if (host)
                SendMessageFromHost(server, message);
            else
                SendMessageToServer(server, message);
        }

        /// <summary>
        /// Tell each client that the dice has been thrown and show the result
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="diceResult">The result of the dice throw</param>
        /// <param name="host">True if the sender of the message is the host</param>
        public static void ThrownDice(Server server, int diceResult, bool host)
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
        /// Request the <see cref="Server"/> to change the turn
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        public static void RequestChangeOfTurn(Server server)
        {
            // CTR stands for "CHANGE TURN REQUEST", this is used 
            // to identify what type of message has been recieved/sent
            string message = "CTR";
            SendMessageToServer(server, message);
        }

        /// <summary>
        /// Tell the other clients which <see cref="Player"/>'s turn it is
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="player">The <see cref="Player"/> whose turn it is</param>
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
        /// Send over a client's <see cref="Player"/> object to all other clients
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="player">The <see cref="Player"/> object to send over</param>
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
        /// Informs all clients of maximum amount of <see cref="Player">players</see>
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message on</param>
        /// <param name="maxPlayers">The max amount of <see cref="Player">players</see></param>
        public static void SendMaxPlayers(Server server, int maxPlayers)
        {
            // SMP stands for "SEND MAX PLAYERS", this is used
            // to identify what type of message has been recieved/sent
            string message = $"SMP|{maxPlayers}";
            SendMessageFromHost(server, message);
        }

        /// <summary>
        /// Send over a client's <see cref="Player"/> object to all other clients, also inform the client
        /// of the total number of <see cref="Player"/> objects that will be sent
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="player">The <see cref="Player"/> object to send over</param>
        /// <param name="count">This keeps track of how many <see cref="Player"/> objects will be sent over in total</param>
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
        /// Tell all other clients which <see cref="Player">players</see> are ready and which aren't
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="players">The list of <see cref="Player">players</see> connected to the server</param>
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
        /// Begin recieving data from the <see cref="Server"/>
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to recieve data from</param>
        /// <param name="host">Whether or not the caller of the method is the host of the <see cref="Server"/></param>
        public static void BeginListeningForMessages(Server server, bool host)
        {
            if (host)
            {
                for(int i = 0; i < server.clients.Count; i++)
                    RecieveData(server, server.clients[i]);
            }
            else
            {
                RecieveDataFromServer(server);
            }
        }

        /// <summary>
        /// Recieve data sent to the <see cref="Server"/> and broadcast it to all clients
        /// </summary>
        /// <param name="server">The <see cref="Server"/> to broadcast the message to</param>
        /// <param name="client">The <see cref="TcpClient"/> that will listen to messages</param>
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
                MessageBox.Show(err.Message, "Serverfel", MessageBoxButtons.OK, MessageBoxIcon.Error); return; 
            }

            string message = Encoding.UTF8.GetString(buffer, 0, n);
            if (message != null && message != "")
            {
                // Broadcast the message
                foreach (TcpClient clt in server.clients)
                {
                    // Make sure NOT to send the message back to origin UNLESS it's the users' ready status
                    if (!clt.Equals(client) || $"{message[0]}{message[1]}{message[2]}".Equals("SRS"))
                    {
                        SendMessage(clt, message);
                    }
                }

                // Send the message to oneself (I.e. the server)
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
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "MVC": // A character has been moved
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "HAW": // A player has won
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "TRD": // The dice have been thrown
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "CTR": // A request to change the turn has been sent
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "SMP": // Send an number informing the clients of the max amount of players
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "ISF": // A client wishes to know if the server is full
                            new FrmMenu().HandleMessageRecievedByServer(message);
                        break;
                    case "INA": // A client wishes to know if a name is available
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SPD": // Player data has been sent
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SPN": // Player data without a list of characters has been sent
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SRS": // Ready status of all players have been sent
                        if (server.form is FrmMenu)
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
                MessageBox.Show(err.Message, "Serverfel", MessageBoxButtons.OK, MessageBoxIcon.Error); return; 
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
                    case "LTL": // A message to all clients telling them to leave the game back to lobby
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "SDG": // A message to all clients informing them that the host has stopped the server
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "FDP": // Player forcefully disconnected
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "MVC": // A character has been moved
                        if(server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "HAW": // A player has won
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "TRD": // The dice have been thrown
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "CHT": // The turn has been changed
                        if (server.form is FrmGame)
                            (server.form as FrmGame).HandleMessageRecievedByServer(message);
                        break;
                    case "SMP": // Send an number informing the clients of the max amount of players
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SFR": // A result from the "Is the server full?" request has been recieved
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "NAR": // A result from the "Is this name available?" request has been recieved
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SPD": // Player data has been sent
                        if (server.form is FrmMenu)
                            (server.form as FrmMenu).HandleMessageRecievedByServer(message);
                        break;
                    case "SRS": // Ready status of all players have been sent
                        if (server.form is FrmMenu)
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
            catch (Exception err) { MessageBox.Show($"Kunde ej skicka meddelande till servern.\n{err.Message}", "Serverfel", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

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
            catch (Exception err) { MessageBox.Show($"Kunde ej skicka meddelande till klienten.\n{err.Message}", "Serverfel", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
        }

        public override string ToString()
        {
            if (this.ip != null)
                return $"{this.ip}:{this.port}";
            else
                return $"Port: {this.port}";
        }

        public TcpClient Client
        {
            private set { this.client = value; }
            get
            {
                return this.client;
            }
        }

        public Form Form
        { 
            get { return this.form; }
            set { this.form = value; }
        }

        public int Port
        {
            get { return this.port; }
        }
    }
}