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

        // Important for client
        private static Server server;
        private static Player player;
        private static List<Character> characters;
        private static ServerType serverType = ServerType.NOT_HOSTING;

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
                            if(await Server.JoinServer(server))
                            {
                                serverType = ServerType.NOT_HOSTING;

                                // Create a Player object and send it to the server
                                player = new Player(ltbNameJoin.Text, Character.Assign(Color.Red), Player.PlayerState.NOT_READY);
                                players.Add(player);
                                UpdatePlayerList();
                                Server.SendPlayerData(server, player, serverType != 0);
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
                        player = new Player(ltbNameJoin.Text, Character.Assign(Color.Red), Player.PlayerState.NOT_READY);
                        players.Add(player);
                        UpdatePlayerList();
                        Server.SendPlayerData(server, player, serverType != 0);
                    }
                }
            }
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if(btnStartServer.Text.Equals("STARTA SERVERN")) // Start the server
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

                                // Disable the server configuration textboxes
                                ltbNameHost.Enabled = false;
                                ltbMaxPlayers.Enabled = false;
                                tbxPortHost.Enabled = false;

                                // Disable the "Back" button
                                btnBack.Enabled = false;

                                // Create a player for the host
                                characters = Character.Assign(Color.Yellow);
                                player = new Player(ltbNameHost.Text, characters, Player.PlayerState.READY);
                                players.Add(player);
                                UpdatePlayerList();

                                // Inform all other clients of the player having connected
                                /* MIGHT ACTUALLY BE UNNESSECARY, SINCE THE HOST IS ALWAYS THE FIRST
                                PLAYER TO CONNECT */
                                Server.SendPlayerData(host, player, true);
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

                // Enable the server configuration textboxes
                ltbNameHost.Enabled = true;
                ltbMaxPlayers.Enabled = true;
                tbxPortHost.Enabled = true;

                // Enable the "Back" button
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
                pbx.Image = pbx.Tag.Equals("ready") ? Properties.Resources.ready : Properties.Resources.not_ready;
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
                    ((PictureBox)pbx[0]).Image = Properties.Resources.not_ready;
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
            if ((Regex.IsMatch(ltbMaxPlayers.Text, ".+") && !ltbMaxPlayers.Text.Equals(ltbMaxPlayers.Label)) && (Regex.IsMatch(ltbNameHost.Text, ".+") && !ltbNameHost.Text.Equals(ltbNameHost.Label)))
                btnStartServer.Enabled = true;
            else
                btnStartServer.Enabled = false; 
        }

        /// <summary>
        /// Makes sure to turn on the button "btnConnect" only when both textboxes are filled out
        /// </summary>
        private void JoinTextBoxes_TextChanged(object sender, EventArgs e)
        {
            if ((Regex.IsMatch(ltbServerIP.Text, ".+") && !ltbServerIP.Text.Equals(ltbServerIP.Label)) && (Regex.IsMatch(ltbNameJoin.Text, ".+") && !ltbNameJoin.Text.Equals(ltbNameJoin.Label)))
                btnConnect.Enabled = true;
            else
                btnConnect.Enabled = false;
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
                    pbx.Image = Properties.Resources.ready;
                    pbx.Tag = "ready";
                }
                else
                {
                    pbx.Image = Properties.Resources.not_ready;
                    pbx.Tag = "not_ready";
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
            string[] data;
            switch (msgType)
            {
                case "SPD": // Player data has been sent
                    data = message.Split('|');

                    // Create a Player object and add it to the list of players
                    if (Character.TryParseColour(data[2], out Color colour))
                    {
                        string name = data[1];
                        List<Character> tempChars = Character.Assign(colour);
                            
                        players.Add(new Player(data[1], tempChars, (Player.PlayerState)int.Parse(data[3])));
                        UpdatePlayerList();
                    }
                    else
                    {
                        MessageBox.Show("Inkorrekt data överskickad från servern", "Fel vid överskickning av data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "SRS": // Ready status of all players have been sent
                    data = message.Split('|');

                    for(int i = 0; i < (data.Length / 2); i++)
                    {
                        foreach(Player p in players)
                        {
                            if(p.Name == data[i])
                            {
                                p.State = (Player.PlayerState)int.Parse(data[i + 1]);
                            }
                        }
                    }

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
