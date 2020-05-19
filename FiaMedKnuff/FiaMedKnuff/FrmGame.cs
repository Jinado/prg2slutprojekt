using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FiaMedKnuff.Properties;

namespace FiaMedKnuff
{
    public partial class FrmGame : Form
    {
        public List<Player> players = new List<Player>();
        public Player player = null;
        public int maxPlayers = 0;
        public FrmMenu.ServerType serverType = FrmMenu.ServerType.NOT_HOSTING;
        public Server server = null;

        public FrmGame()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This methods handles the messages recieved from the server
        /// </summary>
        public void HandleMessageRecievedByServer(string message)
        {
            string msgType = $"{message[0]}{message[1]}{message[2]}";
            string[] data = message.Split('|');
            switch (msgType)
            {
                case "PLD": // Player disconnected
                    break;
                case "FDP": // The player has been forcefully disconnected
                    break;
                case "MVC": // A character has been moved
                    break;
                case "HAW": // A player has won
                    break;
                case "TRD": // The dice have been thrown
                    break;
                case "CHT": // The turn has been changed

                    foreach (Player p in players)
                    {
                        if (p.Name.Equals(data[1]))
                        {
                            p.PlayersTurn = true;

                            // Find the label of the current player and colour it Magenta
                            Color colour = p.Characters[0].Colour;
                            string[] str = colour.ToString().Split('[');
                            string final = str[1].Trim(']');
                            Control[] control = Controls.Find($"lblPlayer{final}", true);
                            ((Label)control[0]).ForeColor = Color.Magenta;

                            break;
                        }
                    }

                    break;
            }
        }

        private void FrmGame_Shown(object sender, EventArgs e)
        {
            if(serverType == FrmMenu.ServerType.NOT_HOSTING)
                Server.StartRecievingDataFromServer(server);

            Game.Initialize(players, maxPlayers);
            foreach(Player p in players)
            {
                if (p.Characters[0].Colour.Equals(Color.Green))
                    lblPlayerGreen.Text = p.Name;
                else if (p.Characters[0].Colour.Equals(Color.Yellow))
                    lblPlayerYellow.Text = p.Name;
                else if (p.Characters[0].Colour.Equals(Color.Red))
                    lblPlayerRed.Text = p.Name;
                else if (p.Characters[0].Colour.Equals(Color.Blue))
                    lblPlayerBlue.Text = p.Name;
            }

            if(serverType == FrmMenu.ServerType.HOSTING) 
            {
                Server.ChangeTurn(server, Game.CurrentTurn, serverType != 0);
                foreach(Player p in players)
                {
                    if (p.Equals(Game.CurrentTurn))
                    {
                        p.PlayersTurn = true;

                        // Find the label of the current player and colour it Magenta
                        Color colour = p.Characters[0].Colour;
                        string[] str = colour.ToString().Split('[');
                        string final = str[1].Trim(']');
                        Control[] control = Controls.Find($"lblPlayer{final}", true);
                        ((Label)control[0]).ForeColor = Color.Magenta;

                        break;
                    }
                }
            }
        }

        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox pbx = (PictureBox)sender;

            int pos = int.Parse(pbx.Name.Substring(3));
            Color colour = Game.Squares[pos].Colour;
            if (colour.Equals(Color.Green))
                pbx.Image = pbx.Image = pbx.Tag.Equals("borderless") ? Resources.green_path_border : Resources.green_path;
            else if (colour.Equals(Color.Yellow))
                pbx.Image = pbx.Tag.Equals("borderless") ? Resources.yellow_path_border : Resources.yellow_path;
            else if (colour.Equals(Color.Red))
                pbx.Image = pbx.Tag.Equals("borderless") ? Resources.red_path_border : Resources.red_path;
            else if (colour.Equals(Color.Blue))
                pbx.Image = pbx.Tag.Equals("borderless") ? Resources.blue_path_border : Resources.blue_path;

            if (pbx.Tag.Equals("borderless"))
                pbx.Tag = "border";
            else
                pbx.Tag = "borderless";
        }
    }
}
