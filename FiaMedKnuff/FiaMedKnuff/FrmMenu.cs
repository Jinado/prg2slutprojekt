﻿using FiaMedKnuff.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public partial class FrmMenu : Form
    {
        private enum ServerType { NOT_HOSTING, HOSTING };

        // Important for server
        private static Server host;
        private static List<Player> players = new List<Player>(4);
        private static int maxPlayers;
        private static Color[] availableColours = { Color.Yellow, Color.Red, Color.Blue, Color.Green };
        private static int colourIndex = 0;

        // Important for client
        private static Server server;
        private static Player player;
        private static List<Character> characters;
        private static ServerType serverType = ServerType.NOT_HOSTING;
        private static int spdCount = 0;

        public FrmMenu()
        {
            InitializeComponent();
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
                                server = new Server(fullAddress[0], this, port);
                                if (await Server.JoinServer(server))
                                {
                                    serverType = ServerType.NOT_HOSTING;

                                    // Create a Player object and send it to the server
                                    player = new Player(ltbNameJoin.Text, Player.PlayerState.NOT_READY);
                                    players.Add(player);
                                    Server.SendPlayerData(server, player, serverType != 0);

                                    btnConnect.Text = "LÄMNA";
                                    btnStartServer.Enabled = false;
                                    btnBack.Enabled = false;
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
                        server = new Server(ltbServerIP.Text, this);
                        if (await Server.JoinServer(server))
                        {
                            serverType = ServerType.NOT_HOSTING;

                            // Create a Player object and send it to the server
                            player = new Player(ltbNameJoin.Text, Player.PlayerState.NOT_READY);
                            players.Add(player);
                            Server.SendPlayerData(server, player, serverType != 0);

                            btnConnect.Text = "LÄMNA";
                            btnStartServer.Enabled = false;
                            btnBack.Enabled = false;
                        }
                    }
                }
            }
            else // Disconnect from the server
            {
                btnConnect.Text = "ANSLUT";
                btnBack.Enabled = true;
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
                                characters = Character.Assign(availableColours[colourIndex++]);
                                player = new Player(ltbNameHost.Text, characters, Player.PlayerState.READY);
                                players.Add(player);
                                UpdatePlayerList();
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
                // Stop the server and clear the player list
                Server.Stop(host);
                ClearServer("Host");
                players.Clear();

                // Hide amount of connected players
                lblConnectedPlayersHost.Text = "";

                // Reset the colour index variable
                colourIndex = 0;

                // Enable the server configuration textboxes
                ltbNameHost.Enabled = true;
                ltbMaxPlayers.Enabled = true;
                tbxPortHost.Enabled = true;

                // Enable the "Back"
                btnBack.Enabled = true;

                // Make the server startabel again
                btnStartServer.Text = "STARTA SERVERN";
            }
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

            for(int i = 0; i < players.Count; i++)
            {
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
        /// <param name="type">Whether it is the host or the join part that should be reset</param>
        private void ClearServer(string type)
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
                ClearServer("Host");
                ClearServer("Join");
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
                new FrmGame().ShowDialog();
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
                case "SMP": // Data about the max amount of players has been sent
                    maxPlayers = int.Parse(data[1]);
                    lblConnectedPlayersJoin.Text = $"Antal spelare: {players.Count()}/{maxPlayers}";
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
                        List<Character> tempCharsSPN = Character.Assign(availableColours[colourIndex++]);

                        players.Add(new Player(data[1], tempCharsSPN, (Player.PlayerState)int.Parse(data[2])));
                        lblConnectedPlayersHost.Text = $"Antal spelare: {players.Count}/{maxPlayers}";
                        UpdatePlayerList();

                        // Broadcast the player list to all other clients
                        foreach (Player p in players)
                        {
                            Server.SendPlayerData(host, p, players.Count);
                            Server.SendMaxPlayers(host, maxPlayers);
                        }
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
            if(host != null) // Check if there even is a server currently running
            {
                Server.Stop(host);
                ClearServer("both");
            }
        }
    }
}
