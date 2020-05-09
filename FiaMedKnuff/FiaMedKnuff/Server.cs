using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public class Server
    {
        private TcpListener listener;
        private List<TcpClient> clients;
        private TcpClient client;
        private int port;
        private int maxPlayers;
        private IPAddress ip;

        /// <summary>
        /// This constructor is used for constructing a server to host
        /// </summary>
        /// <param name="listener"></param>
        /// <param name="clients"></param>
        /// <param name="port"></param>
        /// <param name="maxPlayers"></param>
        public Server(TcpListener listener, List<TcpClient> clients, int port, int maxPlayers)
        {
            if (maxPlayers < 2 || maxPlayers > 4)
                throw new InvalidValueOfMaximumPlayersException();

            this.listener = listener;
            this.clients = clients;
            this.port = port;
            this.maxPlayers = maxPlayers;
        }

        /// <summary>
        /// This constructor is used for constructing a server to join
        /// </summary>
        /// <param name="client"></param>
        /// <param name="port"></param>
        public Server(TcpClient client, string ip, int port)
        {
            try
            {
                this.ip = IPAddress.Parse(ip);
            }
            catch(Exception err)
            {
                if(err.InnerException is ArgumentNullException)
                    MessageBox.Show("The IP-address cannot be null", "Invalid IP-address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (err.InnerException is FormatException)
                    MessageBox.Show("The supplied IP-address if formatted incorrectly.", "Invalid IP-address", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show(err.Message, "Invalid IP-address", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            this.client = client;
            this.port = port;
        }

        /// <summary>
        /// Start a server to host a game on
        /// </summary>
        /// <param name="server">The server to host the game on</param>
        static public void StartServer(Server server)
        {
            try
            {
                server.listener = new TcpListener(IPAddress.Any, server.port);
                server.listener.Start();
            }
            catch (Exception err) { MessageBox.Show(err.Message, "Error on server launch", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            ListenForConnections(server);
        }

        /// <summary>
        /// Listens for connections from clients
        /// </summary>
        /// <param name="server">The server to listen on</param>
        private async static void ListenForConnections(Server server)
        {
            TcpClient tempClient;

            try
            {
                tempClient = await server.listener.AcceptTcpClientAsync();
                server.clients.Add(tempClient);
            }
            catch (Exception err) { MessageBox.Show(err.Message, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            //FetchMessages(tempClient);

            // Make sure no more than four clients may join
            if(server.clients.Count != server.maxPlayers)
                ListenForConnections(server);
        }

        /// <summary>
        /// Join a server
        /// </summary>
        /// <param name="server">The server to join</param>
        /// <returns>True if the connections was successful</returns>
        public async static Task<bool> JoinServer(Server server)
        {
            try
            {
                await server.client.ConnectAsync(server.ip, server.port);

                if (server.client.Connected)
                    return true;
            }
            catch (Exception err) { MessageBox.Show(err.Message, "Connection error", MessageBoxButtons.OK, MessageBoxIcon.Error); return false; }

            return false;
        }

        /// <summary>
        /// Disconnect a player from a server
        /// </summary>
        /// <param name="player">The player to disconnect</param>
        /// <param name="server">The server to disconnect from</param>
        public static void Disconnect(Player player, Server server)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        /// <param name="server">The server to stop</param>
        public static void Stop(Server server)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inform all other clients on the server of a move
        /// </summary>
        /// <param name="server">The server to inform</param>
        /// <param name="square">The square to move a character to</param>
        /// <param name="character">The character to move</param>
        public static void MoveCharacter(Server server, Square square, Character character)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Inform all other clients which player has won
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="player">The player that has won the game</param>
        public static void HasWon(Server server, Player player)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tell each client that the dice has been thrown and show the result
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="diceResult">The result of the dice throw</param>
        public static void ThrownDice(Server server, byte diceResult)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tell the other clients which player's turn it is
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="player">The player whose turn it is</param>
        public static void ChangeTurn(Server server, Player player)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send over a client's player object to all other clients
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="player">The player object to send over</param>
        public static void SendPlayerData(Server server, Player player)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Tell all other clients which players are ready and which aren't
        /// </summary>
        /// <param name="server">The server to broadcast the message to</param>
        /// <param name="players">The list of players connected to the server</param>
        public static void SendReadyStatus(Server server, List<Player> players)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Recieve data broadcasted to the clients
        /// </summary>
        /// <param name="server">The server to recieve data from</param>
        public async static void RecieveData(Server server)
        {
            byte[] buffer = new byte[1024];

            int n = 0;
            try
            {
                n = await server.client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            }
            catch (Exception err) { MessageBox.Show(err.Message, "Server error", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            string message = Encoding.UTF8.GetString(buffer, 0, n);
            // I'll have to handle the data here, MAKE SURE TO USE A MAGIC NUMBER AT THE BEGINNING OF THE STRING
            // FOR EASY IDENTIFICATION

            RecieveData(server);
        }
    }
}
