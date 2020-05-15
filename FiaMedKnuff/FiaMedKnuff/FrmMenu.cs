using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public partial class FrmMenu : Form
    {
        private Server host;
        private Server server;
        private List<Player> players = new List<Player>(4);

        private Player player;
        private List<Character> characters;

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

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            if(btnStartServer.Text.Equals("STARTA SERVERN")) // Start the server
            {
                if(int.TryParse(tbxPortHost.Text, out int port))
                {
                    if(port > 1023)
                    {
                        if(int.TryParse(ltbMaxPlayers.Text, out int maxPlayers))
                        {
                            if(maxPlayers >= 2 && maxPlayers <= 4)
                            {
                                host = new Server(maxPlayers, port);
                                Server.StartServer(host);
                                btnStartServer.Text = "STOPPA SERVERN";
                                SetupServer();
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
                ClearServer("Host");
                btnStartServer.Text = "STARTA SERVERN";
            }
        }

        /// <summary>
        /// This method is called to set up the server after it has successfully been started
        /// </summary>
        private void SetupServer()
        {
            characters = Character.Assign(Color.Yellow);
            player = new Player(ltbNameHost.Text, characters, Player.PlayerState.READY);
            players.Add(player);

            Server.SendPlayerData(host, player);

            lblPlayer1Host.Text = player.Name;
            lblPlayer1Host.ForeColor = Color.Gold;
            pbxPlayer1Host.Image = Properties.Resources.ready;
            pbxPlayer1Host.Tag = "ready";
            pbxPlayer1Host.Visible = true;
        }

        /// <summary>
        /// Resets everything when someone stops the server
        /// </summary>
        /// <param name="type">Wheter it is the host or the join part that should be reset</param>
        private void ClearServer(string type)
        {
            if (!type.Equals("both"))
            {
                for (int i = 1; i < 5; i++)
                {
                    Control[] lbl = Controls.Find("lblPlayer" + i + type, true);
                    ((Label)lbl[0]).Text = "";

                    Control[] pbx = Controls.Find("pbxPlayer" + i + type, true);
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
        /// Makes sure to turn on the button "btnStartServer" only when both textboxes are filled out
        /// </summary>
        private void HostTextBoxes_TextChanged(object sender, EventArgs e)
        {
            if ((Regex.IsMatch(ltbMaxPlayers.Text, ".+") && !ltbMaxPlayers.Text.Equals(ltbMaxPlayers.Label)) && (Regex.IsMatch(ltbNameHost.Text, ".+") && !ltbNameHost.Text.Equals(ltbNameHost.Label)))
                btnStartServer.Enabled = true;
            else
                btnStartServer.Enabled = false; 
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
            }
        }

        /// <summary>
        /// This methods handles the message recieved from the server
        /// </summary>
        public void HandleMessageRecievedByServer(string message)
        {
            string msgType = $"{message[0]}{message[1]}{message[2]}";
            switch (msgType)
            {
                case "PLD": // Player disconnected
                    break;
                case "MVC": // A character has been moved
                    break;
                case "HAW": // A player has won
                    break;
                case "TRD": // The dice have been thrown
                    break;
                case "CHT": // The turn has been changed
                    break;
                case "SPD": // Player data has been sent
                    string[] data = message.Split('|');

                    // Create a Player object and add it to the list of players
                    if (Character.TryParseColour(data[2], out Color colour))
                    {
                        string name = data[1];
                        List<Character> tempChars = Character.Assign(colour);
                            
                        players.Add(new Player(data[1], tempChars, (int.Parse(data[3]) == 1) ? Player.PlayerState.READY : Player.PlayerState.NOT_READY));
                    }
                    else
                    {
                        MessageBox.Show("Inkorrekt data överskickad från servern", "Fel vid överskickning av data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;
                case "SRS": // Ready status of all players have been sent
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
