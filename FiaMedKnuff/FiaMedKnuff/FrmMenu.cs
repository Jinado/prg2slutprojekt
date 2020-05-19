using FiaMedKnuff.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public partial class FrmMenu : Form
    {
        public enum ServerType { NOT_HOSTING, HOSTING };

        // Important for server
        private static Server host;
        private static List<Player> players = new List<Player>(4);
        private static int maxPlayers;
        private static readonly Color[] availableColours = { Color.Yellow, Color.Red, Color.Blue, Color.Green };
        private static List<Color> usedColours = new List<Color>(4);

        // Important for client
        private static Server server;
        private static Player player;
        private static List<Character> characters;
        private static ServerType serverType = ServerType.NOT_HOSTING;
        private static int spdCount = 0;
        private static int gamesWon = 0;
        private static int gamesLost = 0;
        private static TcpClient client;

        public FrmMenu()
        {
            InitializeComponent();
        }


        private void FrmMenu_Shown(object sender, EventArgs e)
        {
            try
            {
                string ip = FileHandler.ReadConfigData();
                ltbServerIP.Text = ip;
                ltbServerIP.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            }
            catch(Exception) { /* Do nothing because the file does not yet exist */ }
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = Color.IndianRed;
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            ((Button)sender).BackColor = Color.SlateBlue;
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();
            new FrmMain().ShowDialog();
            this.Close();
        }

        /// <summary>
        /// Connect a user to the server, if the user is already connected, disconnect him
        /// </summary>
        private async void btnConnect_Click(object sender, EventArgs e)
        {
            if (btnConnect.Text.Equals("ANSLUT"))
            {
                // Make sure the name is does not include a | because that will interfer with
                // the string when sending it over the network
                if (ltbNameJoin.Text.Contains('|'))
                {
                    MessageBox.Show("Teckent '|' får ej finnas med i namnet.", "Olagligt tecken", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ltbNameJoin.Text = ltbNameJoin.Text.Replace("|", "");
                    return;
                }

                // Check if the IP-address entered contains a port number
                int port = 6767;
                if (ltbServerIP.Text.Contains(":"))
                {
                    // Make sure the port is valid
                    string[] fullAddress = ltbServerIP.Text.Split(':');
                    if (!int.TryParse(fullAddress[1], out port))
                    {
                        MessageBox.Show("Ogiltig IP-adress. Om du har med ett kolon måste en fyrsiffrig port som INTE finns i intervallet 0 - 1023 stå direkt efter.", "Ogiltig IP-adress", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        if (port < 1024)
                        {
                            MessageBox.Show("Ogiltig IP-adress. Om du har med ett kolon måste en fyrsiffrig port som INTE finns i intervallet 0 - 1023", "Ogiltig IP-adress", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else // The port is valid, check if the IP-address is too
                        {
                            if (IPAddress.TryParse(fullAddress[0], out IPAddress _))
                            {
                                client = new TcpClient();
                                server = new Server(client, fullAddress[0], this, port);
                                if (await Server.JoinServer(server))
                                {
                                    serverType = ServerType.NOT_HOSTING;

                                    // Create a Player object and send it to the server
                                    player = new Player(ltbNameJoin.Text.Replace(" ", ""), Player.PlayerState.NOT_READY);

                                    // Ask the server if the name is available
                                    Server.IsNameAvailable(server, player.Name);

                                    // Disable buttons and change the connect button to a disconnect button
                                    btnConnect.Text = "LÄMNA";
                                    btnStartServer.Enabled = false;
                                    btnBack.Enabled = false;

                                    // Disable the input fields
                                    ltbNameJoin.Enabled = false;
                                    ltbServerIP.Enabled = false;

                                    // Save IP because connection was successful
                                    FileHandler.SaveConfigData($"{fullAddress[0]}:{fullAddress[1]}");

                                    // Load or save user data
                                    HandleUserData();
                                }
                            }
                            else
                            {
                                MessageBox.Show("Ogiltig IP-adress. IP-adressen är skriven i fel format", "Ogiltig IP-adress", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (IPAddress.TryParse(ltbServerIP.Text, out IPAddress _))
                    {
                        client = new TcpClient();
                        server = new Server(client, ltbServerIP.Text, this);
                        if (await Server.JoinServer(server))
                        {
                            serverType = ServerType.NOT_HOSTING;

                            // Create a Player object and send it to the server
                            player = new Player(ltbNameJoin.Text.Replace(" ", ""), Player.PlayerState.NOT_READY);

                            // Ask the server if the name is available
                            Server.IsNameAvailable(server, player.Name);

                            // Disable buttons and change the connect button to a disconnect button
                            btnConnect.Text = "LÄMNA";
                            btnStartServer.Enabled = false;
                            btnBack.Enabled = false;

                            // Disable the input fields
                            ltbNameJoin.Enabled = false;
                            ltbServerIP.Enabled = false;

                            // Save IP because connection was successful
                            FileHandler.SaveConfigData(ltbServerIP.Text);

                            // Load or save user data
                            HandleUserData();
                        }
                    }
                }
            }
            else // Disconnect from the server
            {
                // Send a disconnect message to the server
                Server.Disconnect(player, server, serverType != 0);
                ResetFormOnDisconnect();
            }
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            // Make sure the name is does not include a | because that will interfer with
            // the string when sending it over the network
            if (ltbNameHost.Text.Contains('|'))
            {
                MessageBox.Show("Teckent '|' får ej finnas med i namnet.", "Olagligt tecken", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ltbNameHost.Text = ltbNameHost.Text.Replace("|", "");
                return;
            }

            if (btnStartServer.Text.Equals("STARTA SERVERN")) // Start the server
            {
                if(int.TryParse(tbxPortHost.Text, out int port))
                {
                    if(port > 1023)
                    {
                        if(int.TryParse(ltbMaxPlayers.Text, out maxPlayers))
                        {
                            if(maxPlayers >= 2 && maxPlayers <= 4)
                            {
                                // Start the server and allow the server to be stopped
                                host = new Server(maxPlayers, this, port);
                                Server.StartServer(host);
                                btnStartServer.Text = "STOPPA SERVERN";
                                serverType = ServerType.HOSTING;

                                // Display amount of connected players
                                lblConnectedPlayersHost.Text = $"Antal spelare: 1/{maxPlayers}";

                                // Disable the server configuration textboxes
                                ltbNameHost.Enabled = false;
                                ltbMaxPlayers.Enabled = false;
                                tbxPortHost.Enabled = false;

                                // Disable the "Back" and "Connect" button
                                btnBack.Enabled = false;
                                btnConnect.Enabled = false;

                                // Create a player for the host
                                characters = Character.Assign(FindAvailableColour());
                                player = new Player(ltbNameHost.Text.Replace(" ", ""), characters, Player.PlayerState.READY);
                                players.Add(player);
                                UpdatePlayerList();

                                // Load or save user data
                                HandleUserData();
                            }
                            else
                            {
                                MessageBox.Show("Antal spelare måste vara minst 2 och max 4", "Inkorrekt antal spelare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Antal spelare måste vara minst 2 och max 4", "Inkorrekt antal spelare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Du måste skriva en port som är antingen 1024 eller högre", "Inkorrekt port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        tbxPortHost.Text = "6767";
                    }
                }
                else
                {
                    MessageBox.Show("Du måste skriva en port som är antingen 1024 eller högre", "Inkorrekt port", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tbxPortHost.Text = "6767";
                }
            }
            else // Stop the server
            {
                Server.Stop(host);
                ResetFormOnStop();
            }
        }

        /// <summary>
        /// Try to load the data of a connected user, if there is no data to load, save the data instead
        /// </summary>
        private void HandleUserData()
        {
            try
            {
                if (serverType == ServerType.HOSTING)
                {
                    FileHandler.ReadUserData(ltbNameHost.Text, ref gamesWon, ref gamesLost);
                    lblUserDataHost.Text = $"Vunnit: {gamesWon} | Förlorat: {gamesLost}";
                }
                else
                {
                    FileHandler.ReadUserData(ltbNameJoin.Text, ref gamesWon, ref gamesLost);
                    lblUserDataJoin.Text = $"Vunnit: {gamesWon} | Förlorat: {gamesLost}";
                }
            }
            catch (Exception err)
            {
                if (err is FileNotFoundException)
                {
                    try
                    {
                        if (serverType == ServerType.HOSTING)
                        {
                            FileHandler.SaveUserData(ltbNameHost.Text, gamesWon, gamesLost);
                            lblUserDataHost.Text = $"Vunnit: {gamesWon} | Förlorat: {gamesLost}";
                        }
                        else
                        {
                            FileHandler.SaveUserData(ltbNameJoin.Text, gamesWon, gamesLost);
                            lblUserDataJoin.Text = $"Vunnit: {gamesWon} | Förlorat: {gamesLost}";
                        }
                    }
                    catch (Exception) { /* Do nothing */ }
                }
            }
        }

        /// <summary>
        /// Finds an available colour to be assigned to a player
        /// </summary>
        /// <returns>The colour to be used for the player's characters</returns>
        private Color FindAvailableColour()
        {
            if(usedColours.Count != 0)
            {
                int i = 0;
                foreach (Color ac in availableColours)
                {
                    do
                    {
                        // If you find the colour inside the used colours list, break out
                        // of the do-while loop and look at the next colour
                        if (usedColours[i].Equals(ac))
                        {
                            i = 0;
                            break;
                        }

                        // If this is true, none of the colours in the used colours list matched
                        // the searched colour (ac)
                        if (i == (usedColours.Count - 1))
                        {
                            usedColours.Add(ac);
                            return ac;
                        }

                        i++;
                    } while (i < usedColours.Count);

                    i = 0;
                }
            }
            else
            {
                int rnd = new Random().Next(4);
                usedColours.Add(availableColours[rnd]);
                return availableColours[rnd];
            }
            

            return Color.White;
        }

        /// <summary>
        /// This method is called each time a player gets added to the list of players
        /// </summary>
        private void UpdatePlayerList()
        {
            // Makes sure to target the correct labels and pictureboxes
            string srvType = "Join";
            if (serverType == ServerType.HOSTING)
                srvType = "Host";

            ClearPlayerList(srvType);

            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Characters == null) continue;

                // Colours the labels correctly as well as writes the user's name to them
                Control[] lblAsControl = Controls.Find($"lblPlayer{i + 1}{srvType}", true);
                Label lbl = lblAsControl[0] as Label;
                lbl.Text = players[i].Name;
                lbl.ForeColor = players[i].Characters[0].Colour.Equals(Color.Yellow) ? Color.Gold : players[i].Characters[0].Colour;

                // Makes the ready buttons visible as well as automatically readying up the host of the game
                Control[] pbxAsControl = Controls.Find($"pbxPlayer{i + 1}{srvType}", true);
                PictureBox pbx = pbxAsControl[0] as PictureBox;
                pbx.Tag = players[i].State.Equals(Player.PlayerState.READY) ? "ready" : "not_ready";
                pbx.Image = pbx.Tag.Equals("ready") ? Resources.ready : Resources.not_ready;
                pbx.Visible = true;
            }
        }

        /// <summary>
        /// Resets everything when someone stops the server
        /// </summary>
        /// <param name="type">Whether it is the host, join or both parts that should be reset</param>
        private void ClearPlayerList(string type)
        {
            if (!type.Equals("both"))
            {
                for (int i = 1; i < 5; i++)
                {
                    Control[] lbl = Controls.Find($"lblPlayer{i}{type}", true);
                    ((Label)lbl[0]).Text = "";

                    Control[] pbx = Controls.Find($"pbxPlayer{i}{type}", true);
                    ((PictureBox)pbx[0]).Visible = false;
                    ((PictureBox)pbx[0]).Image = Resources.not_ready;
                }
            }
            else
            {
                ClearPlayerList("Host");
                ClearPlayerList("Join");
            }
        }

        /// <summary>
        /// Sees if each player is ready, and in that case, starts the game
        /// </summary>
        private void CheckReadyStatus()
        {
            // Make sure everyone has connected
            if(players.Count == maxPlayers)
            {
                foreach (Player p in players)
                {
                    if (p.State == Player.PlayerState.NOT_READY) return;
                }

                // If the code got here, everyone is ready
                this.Hide();
                FrmGame frmGame = new FrmGame();

                // Send over the necessary information
                frmGame.players = players;
                frmGame.player = player;
                frmGame.maxPlayers = maxPlayers;
                frmGame.serverType = serverType;
                frmGame.server = serverType == ServerType.HOSTING ? host : server;
                frmGame.server.Form = frmGame;

                // Show the game form
                frmGame.ShowDialog();
                this.Close();
            }
        }

        /// <summary>
        /// Makes sure to turn on the button "btnStartServer" only when both textboxes are filled out
        /// </summary>
        private void HostTextBoxes_TextChanged(object sender, EventArgs e)
        {
            if (btnConnect.Enabled == false && (Regex.IsMatch(ltbMaxPlayers.Text, ".+") && !ltbMaxPlayers.Text.Equals(ltbMaxPlayers.Label)) && (Regex.IsMatch(ltbNameHost.Text, ".+") && !ltbNameHost.Text.Equals(ltbNameHost.Label)))
            {
                btnStartServer.Enabled = true;
            }
            else
            {
                btnStartServer.Enabled = false;

                // Enable the connect button in case it's textboxes are also filled out
                if (btnStartServer.Enabled == false && (Regex.IsMatch(ltbServerIP.Text, ".+") && !ltbServerIP.Text.Equals(ltbServerIP.Label)) && (Regex.IsMatch(ltbNameJoin.Text, ".+") && !ltbNameJoin.Text.Equals(ltbNameJoin.Label)))
                    btnConnect.Enabled = true;
            }
        }

        /// <summary>
        /// Makes sure to turn on the button "btnConnect" only when both textboxes are filled out
        /// </summary>
        private void JoinTextBoxes_TextChanged(object sender, EventArgs e)
        {
            if (btnStartServer.Enabled == false &&  (Regex.IsMatch(ltbServerIP.Text, ".+") && !ltbServerIP.Text.Equals(ltbServerIP.Label)) && (Regex.IsMatch(ltbNameJoin.Text, ".+") && !ltbNameJoin.Text.Equals(ltbNameJoin.Label)))
            {
                btnConnect.Enabled = true;
            }
            else
            {
                btnConnect.Enabled = false;

                // Enable the startServer button in case it's textboxes are also filled out
                if (btnConnect.Enabled == false && (Regex.IsMatch(ltbMaxPlayers.Text, ".+") && !ltbMaxPlayers.Text.Equals(ltbMaxPlayers.Label)) && (Regex.IsMatch(ltbNameHost.Text, ".+") && !ltbNameHost.Text.Equals(ltbNameHost.Label)))
                    btnStartServer.Enabled = true;
            }
        }

        private void tbxPortHost_Leave(object sender, EventArgs e)
        {
            // Checks if the port can be parsed as an int
            if (int.TryParse(tbxPortHost.Text, out int port))
            {
                // Makes sure the int value is greater than 1023
                if (port > 1023) return;
            }

            // If any of the checks above fails, the following code will run
            MessageBox.Show("Du måste skriva en port som är antingen 1024 eller högre", "Inkorrekt port", MessageBoxButtons.OK, MessageBoxIcon.Error);
            tbxPortHost.Text = "6767";
        }

        private void PictureBoxReadyStatus_Click(object sender, EventArgs e)
        {
            PictureBox pbx = (PictureBox)sender;
            if (pbx.Visible)
            {
                if (pbx.Tag.Equals("not_ready"))
                {
                    pbx.Tag = "ready";
                    pbx.Image = Resources.ready;
                    player.State = Player.PlayerState.READY;
                }
                else
                {
                    pbx.Tag = "not_ready";
                    pbx.Image = Resources.not_ready;
                    player.State = Player.PlayerState.NOT_READY;
                }

                // Broadcast the ready status of each player to each client
                Server.SendReadyStatus(serverType == ServerType.HOSTING ? host : server, players, serverType != 0);
            }
        }

        /// <summary>
        /// This methods handles the message recieved from the server
        /// </summary>
        public void HandleMessageRecievedByServer(string message)
        {
            string msgType = $"{message[0]}{message[1]}{message[2]}";
            string[] data = message.Split('|');
            switch (msgType)
            {
                case "PLD": // Player disconnected

                    // Check if the player who disconnected is the same player who recieved the message
                    // (this happens when that player tried to join a full server)
                    if(serverType == ServerType.NOT_HOSTING && data[1].Equals(player.Name))
                    {
                        ResetFormOnDisconnect();
                        MessageBox.Show("Din anslutning blev nekad då servern är full.", "Full server", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    // Loop through the player list and remove the disconnected player
                    Color colourPLD = Color.White;
                    for(int i = 0; i < players.Count; i++)
                    {
                        if(players[i].Name == data[1])
                        {
                            if(serverType == ServerType.HOSTING) colourPLD = players[i].Characters[0].Colour;
                            players.RemoveAt(i);
                            break;
                        }
                    }

                    UpdatePlayerList();

                    // Update the number of connected players
                    if (serverType == ServerType.HOSTING)
                        lblConnectedPlayersHost.Text = $"Anslutna spelare: {players.Count}/{maxPlayers}";
                    else
                        lblConnectedPlayersJoin.Text = $"Anslutna spelare: {players.Count}/{maxPlayers}";

                    // Make sure the next player who joins will get the colour of the player who disconnected
                    if (serverType == ServerType.HOSTING)
                    {
                        for(int i = 0; i < usedColours.Count; i++)
                        {
                            if (usedColours[i].Equals(colourPLD))
                            {
                                usedColours.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    break;
                case "FDP":

                    // Reset the form
                    if (serverType == ServerType.HOSTING)
                        ResetFormOnStop();
                    else
                        ResetFormOnDisconnect();

                    break;
                case "SMP": // Data about the max amount of players has been sent

                    maxPlayers = int.Parse(data[1]);
                    lblConnectedPlayersJoin.Text = $"Antal spelare: {players.Count()}/{maxPlayers}";

                    break;
                case "INA": // A client wishes to know if a name is available

                    // Only handle this event if you are the host of the server
                    if(serverType == ServerType.HOSTING)
                    {
                        foreach(Player p in players)
                        {
                            if (p.Name.Equals(data[1]))
                            {
                                // Inform the clients that the name is not available
                                Server.NameAvailableResult(host, $"{data[1]}|no");
                                return;
                            }
                        }

                        Server.NameAvailableResult(host, $"{data[1]}|yes");
                    }

                    break;
                case "NAR":

                    // Only handle this event if you are NOT the host of the server
                    // and you are the one who sent the request
                    if (serverType == ServerType.NOT_HOSTING && player.Name.Equals(data[1]))
                    {
                        if (data[2].Equals("yes"))
                        {
                            // Since the name is available, add the player and send over the object
                            // to the server
                            players.Add(player);
                            Server.SendPlayerData(server, player, serverType != 0);
                        }
                        else
                        {
                            Server.Disconnect(server);
                            ResetFormOnDisconnect();
                            UpdatePlayerList();

                            MessageBox.Show("Din anslutning blev nekad då det redan finns en spelare med samma namn som ditt ansluten till servern.", "Upptaget namn", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }

                    break;
                case "SPD": // Player data has been sent and it includes a list of characters

                    // Create a Player object and add it to the list of players
                    if (Character.TryParseColour(data[2], out Color colour))
                    {
                        List<Character> tempCharsSPD = Character.Assign(colour);

                        // Make sure the recieved Player object does not already exist in the Player list
                        Player playerSPD = new Player(data[1], tempCharsSPD, (Player.PlayerState)int.Parse(data[3]));
                        foreach(Player p in players)
                        {
                            if (p.Name == playerSPD.Name)
                            {
                                // If the player is the same as the client player, make sure the client's
                                // local variable copies the colour from one sent by the server
                                if (player.Name == playerSPD.Name) player.Characters = playerSPD.Characters;

                                if (data.Length == 5)
                                {
                                    spdCount++;
                                    if (spdCount == int.Parse(data[4]))
                                    {
                                        UpdatePlayerList();
                                        spdCount = 0;
                                    }
                                }

                                return;
                            }
                        }

                        // If the player is unique, add it to the list
                        players.Add(playerSPD);

                        // This makes sure to update the player list ONLY when every player has been sent over
                        // from the server.
                        if(data.Length == 5)
                        {
                            spdCount++;
                            if (spdCount == int.Parse(data[4]))
                            {
                                UpdatePlayerList();
                                spdCount = 0;
                            }
                        }
                        else
                        {
                            UpdatePlayerList();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Inkorrekt data överskickad från servern", "Fel vid överskickning av data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    break;
                case "SPN": // Player data has been sent but it doesn't include a list of characters

                    // Only handle this event if you are the host of the server
                    if(serverType == ServerType.HOSTING)
                    {
                        // Create a Player object and add it to the list of players
                        List<Character> tempCharsSPN = Character.Assign(FindAvailableColour());

                        players.Add(new Player(data[1], tempCharsSPN, (Player.PlayerState)int.Parse(data[2])));

                        // Broadcast the player list to all other clients
                        foreach (Player p in players)
                        {
                            Server.SendPlayerData(host, p, players.Count);
                            Server.SendMaxPlayers(host, maxPlayers);
                        }

                        // Check if there are too many connected players right now
                        if (maxPlayers < players.Count)
                        {
                            // Make sure to wait a little so that the other messages are sent and processed in time
                            Thread.Sleep(50);
                            Server.Disconnect(players[players.Count - 1], host, serverType != 0);
                            Color colourSPN = players[players.Count - 1].Characters[0].Colour;

                            // Remove player from the player list
                            players.RemoveAt(players.Count - 1);

                            // Remove the colour that player was assigned from the usedColour list
                            for (int i = 0; i < usedColours.Count; i++)
                            {
                                if (usedColours[i].Equals(colourSPN))
                                {
                                    usedColours.RemoveAt(i);
                                    break;
                                }
                            }

                        } 

                        lblConnectedPlayersHost.Text = $"Antal spelare: {players.Count}/{maxPlayers}";
                        UpdatePlayerList();
                    }
                    
                    break;
                case "SRS": // Ready status of all players have been sent

                    for(int i = 1; i < ((data.Length / 2) + 1); i++)
                    {
                        foreach(Player p in players)
                        {
                            if(p.Name == data[i])
                            {
                                p.State = (Player.PlayerState)int.Parse(data[i + 1]);
                            }
                        }
                    }

                    UpdatePlayerList();
                    CheckReadyStatus();
                    break;
            }
        }

        private void FrmMenu_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(host != null) // Check if the user is currently hosting a server
            {
                Server.Stop(host);
                ResetFormOnStop();
                return;
            }
            
            if(server != null)
            {
                if (server.Client.Connected) // Check if the user is currently connected to a server
                {
                    // Send a disconnect message to the server
                    Server.Disconnect(player, server, serverType != 0);
                    ResetFormOnDisconnect();
                }

                return;
            }
        }

        private void ResetFormOnStop()
        {
            // Clear the player list
            ClearPlayerList("Host");
            players.Clear();
            usedColours.Clear();

            // Hide amount of connected players and the user's data
            lblConnectedPlayersHost.Text = "";
            lblUserDataHost.Text = "";

            // Enable the server configuration textboxes
            ltbNameHost.Enabled = true;
            ltbMaxPlayers.Enabled = true;
            tbxPortHost.Enabled = true;

            // Enable the "Back"
            btnBack.Enabled = true;

            // Make the server startable again
            btnStartServer.Text = "STARTA SERVERN";
        }

        private void ResetFormOnDisconnect()
        {
            // Change the disconnect button to a connect button
            btnConnect.Text = "ANSLUT";

            // Clear the amount of connected players and clear the Player list
            lblConnectedPlayersJoin.Text = "";
            lblUserDataJoin.Text = "";
            players.Clear();
            player = null;
            ClearPlayerList("Join");

            // Enable the input fields and the back button
            ltbNameJoin.Enabled = true;
            ltbServerIP.Enabled = true;
            btnBack.Enabled = true;
        }
    }
}
