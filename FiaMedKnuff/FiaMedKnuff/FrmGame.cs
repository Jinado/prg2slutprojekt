using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FiaMedKnuff.Properties;

namespace FiaMedKnuff
{
    public partial class FrmGame : Form
    {
        // Data transferred from the FrmMenu form
        public List<Player> players = new List<Player>();
        public Player player = null;
        public int maxPlayers = 0;
        public FrmMenu.ServerType serverType = FrmMenu.ServerType.NOT_HOSTING;
        public Server server = null;

        // Variables useful for this client
        private static int diceResult;
        private static bool diceThrown = false;
        private static bool reThrowAllowed = false;
        private static PictureBox[] paths = new PictureBox[57];
        private static PictureBox selectedCharacter;
        private static int score = 0;

        public FrmGame()
        {
            InitializeComponent();
        }

        private void PictureBoxPath_Click(object sender, EventArgs e)
        {
            Square path = Game.Squares[int.Parse(((PictureBox)sender).Name.Substring(3))];
            // Make sure the user pressed a bordered path
            if (path.Border == Square.SquareBorder.BORDER)
            {
                string number = selectedCharacter.Name.Replace("pbxChar", "").Replace(Character.ColourToString(player.Characters[0].Colour), "");
                int index = int.Parse(number);

                // Get the Character object
                Character character = player.Characters[index];
                Movement.MoveCharacter(path, character, ((PictureBox)sender), selectedCharacter);
                selectedCharacter = null;

                // If the path the character was moved to was the goal path, add one to the player's score
                // and remove the character from the game
                if(path.Path == Square.PathType.GOAL)
                {
                    score++;
                    character.State = Character.CharacterState.WON;
                    selectedCharacter.Enabled = false;
                    selectedCharacter.Visible = false;

                    CheckScore();
                }

                // The user has moved, change the turn unless the user may move again
                if (!reThrowAllowed && player.PlayersTurn)
                {
                    // Change turn
                    Game.ChangeTurn(players);
                    player.PlayersTurn = false;
                    TurnChanged();

                    // Notify the other clients of the new turn
                    Server.ChangeTurn(server, Game.CurrentTurn, serverType != 0);
                }

                UpdateBoard();

                diceThrown = false;
            }
        }

        private void pbxCharacter_Click(object sender, EventArgs e)
        {
            PictureBox pbx = (PictureBox)sender;

            // Make sure you're pressing one of your own characters
            if (pbx.Tag.Equals(Character.ColourToString(player.Characters[0].Colour)))
            {
                // You may only move if you've thrown the dice
                if (diceThrown)
                {
                    // Get the index of the character pressed
                    selectedCharacter = pbx;
                    string number = pbx.Name.Replace("pbxChar", "").Replace(Character.ColourToString(player.Characters[0].Colour), "");
                    int index = int.Parse(number);

                    // Get the Character object
                    Character character = player.Characters[index];

                    // Check if and where a player may move
                    if (Movement.DrawMovementLine(diceResult, character))
                    {
                        UpdateBoard();
                    }
                }
            }
        }


        private void pbxDice_Click(object sender, EventArgs e)
        {
            // Only throw the dice if it is the players turn
            if (player.PlayersTurn && (!diceThrown || reThrowAllowed))
            {
                // Get a result
                diceResult = new Random().Next(7);
                pbxDice.Tag = diceResult;
                Server.ThrownDice(server, diceResult, serverType != 0);

                // May the user throw again?
                reThrowAllowed = diceResult == 6;
                diceThrown = true;

                // "Animate" the dice a little
                pbxDice.Left += 15;
                Thread.Sleep(100);
                pbxDice.Top -= 15;
                Thread.Sleep(100);
                pbxDice.Left -= 10;
                Thread.Sleep(100);
                pbxDice.Top += 18;
                Thread.Sleep(100);
                pbxDice.Left -= 5;
                Thread.Sleep(100);
                pbxDice.Top -= 3;
                Thread.Sleep(100);

                switch (diceResult)
                {
                    case 1:
                        pbxDice.Image = Resources.dice_1;
                        break;
                    case 2:
                        pbxDice.Image = Resources.dice_2;
                        break;
                    case 3:
                        pbxDice.Image = Resources.dice_3;
                        break;
                    case 4:
                        pbxDice.Image = Resources.dice_4;
                        break;
                    case 5:
                        pbxDice.Image = Resources.dice_5;
                        break;
                    case 6:
                        pbxDice.Image = Resources.dice_6;
                        break;
                }

                // Check if and where a player may move
                int noMove = 0;
                foreach(Character c in player.Characters)
                {
                    // Check if the character c, can move. If it cannot, increment noMove by one
                    if (!Movement.DrawMovementLine(diceResult, c, true))
                        noMove++;
                    else break;
                }

                if(noMove == player.Characters.Count)
                {
                    if (reThrowAllowed)
                    {
                        MessageBox.Show("Det finns inget drag att göra men du kan kasta tärningen igen!", "Du har inga drag. Kasta igen!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Det finns inget drag att göra", "Du har inga drag", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        player.PlayersTurn = false;
                        diceThrown = false;
                        Game.ChangeTurn(players);
                        TurnChanged();

                        // Notify the other clients of the new turn
                        Server.ChangeTurn(server, Game.CurrentTurn, serverType != 0);
                    }
                }
            }
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
                case "TRD": // The dice has been thrown

                    // "Animate" the dice a little
                    pbxDice.Left += 15;
                    Thread.Sleep(100);
                    pbxDice.Top -= 15;
                    Thread.Sleep(100);
                    pbxDice.Left -= 10;
                    Thread.Sleep(100);
                    pbxDice.Top += 18;
                    Thread.Sleep(100);
                    pbxDice.Left -= 5;
                    Thread.Sleep(100);
                    pbxDice.Top -= 3;
                    Thread.Sleep(100);

                    diceResult = int.Parse(data[1]);
                    pbxDice.Tag = diceResult;

                    switch (diceResult)
                    {
                        case 1:
                            pbxDice.Image = Resources.dice_1;
                            break;
                        case 2:
                            pbxDice.Image = Resources.dice_2;
                            break;
                        case 3:
                            pbxDice.Image = Resources.dice_3;
                            break;
                        case 4:
                            pbxDice.Image = Resources.dice_4;
                            break;
                        case 5:
                            pbxDice.Image = Resources.dice_5;
                            break;
                        case 6:
                            pbxDice.Image = Resources.dice_6;
                            break;
                    }

                    break;
                case "CHT": // The turn has been changed

                    foreach (Player p in players)
                    {
                        if (p.Name.Equals(data[1]))
                        {
                            p.PlayersTurn = true;
                            Game.CurrentTurn = p;
                            break;
                        }
                    }

                    TurnChanged();

                    break;
            }
        }

        private void FrmGame_Shown(object sender, EventArgs e)
        {
            this.Text += $" : {player.Name}";

            Server.BeginListeningForMessages(server, serverType != 0);

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
                TurnChanged();
            }

            InitializeCharacters();
            InitalizePaths();
        }

        /// <summary>
        /// Colours the current player's name in magenta, and all others' in ControlText
        /// </summary>
        private void TurnChanged()
        {
            foreach (Player p in players)
            {
                // Find the label of the current player and colour it magenta
                string final = Character.ColourToString(p.Characters[0].Colour);
                Control[] control = Controls.Find($"lblPlayer{final}", true);

                if (p.Equals(Game.CurrentTurn))
                {
                    p.PlayersTurn = true;
                    ((Label)control[0]).ForeColor = Color.Magenta;
                }
                else
                {
                    ((Label)control[0]).ForeColor = Color.FromKnownColor(KnownColor.ControlText);
                }
            }
        }

        /// <summary>
        /// Places the pictureboxes representing the characters on the board
        /// </summary>
        private void InitializeCharacters()
        {
            foreach(Player p in players)
            {
                PictureBox[] characters = Character.CreateCharacters(p.Characters[0].Colour);
                foreach (PictureBox pbx in characters)
                {
                    pbx.Click += pbxCharacter_Click;
                    Controls.Add(pbx);
                    Controls.SetChildIndex(pbx, 0);
                }
            }
        }

        /// <summary>
        /// Populates the PictureBox "paths" array with the paths on the board
        /// </summary>
        private void InitalizePaths()
        {
            int added = 0;
            foreach(Control c in Controls)
            {
                if (c is PictureBox)
                {
                    if (Regex.IsMatch(((PictureBox)c).Name, "^pbx[0-9]{1,2}$"))
                    {
                        int index = int.Parse(((PictureBox)c).Name.Substring(3));
                        paths[index] = ((PictureBox)c);
                        added++;
                    }
                }

                if (added == paths.Length) break;
            }
        }

        /// <summary>
        /// Updates the board and shows where a player may move
        /// </summary>
        private void UpdateBoard()
        {
            for(int i = 0; i < paths.Length; i++)
            {
                // Get the colour of the current square
                string colour = Character.ColourToString(Game.Squares[i].Colour);
                if(Game.Squares[i].Border == Square.SquareBorder.BORDER)
                {
                    switch (colour)
                    {
                        case "Green":
                            paths[i].Image = Resources.green_path_border;
                            break;
                        case "Yellow":
                            paths[i].Image = Resources.yellow_path_border;
                            break;
                        case "Red":
                            paths[i].Image = Resources.red_path_border;
                            break;
                        case "Blue":
                            paths[i].Image = Resources.blue_path_border;
                            break;
                        case "Black":
                            paths[i].Image = Resources.goal_border;
                            break;
                    }
                }
                else
                {
                    switch (colour)
                    {
                        case "Green":
                            paths[i].Image = Resources.green_path;
                            break;
                        case "Yellow":
                            paths[i].Image = Resources.yellow_path;
                            break;
                        case "Red":
                            paths[i].Image = Resources.red_path;
                            break;
                        case "Blue":
                            paths[i].Image = Resources.blue_path;
                            break;
                        case "Black":
                            paths[i].Image = Resources.goal;
                            break;
                    }
                }
                    
            }
        }

        /// <summary>
        /// Check the player's score to determine if he has won
        /// </summary>
        private void CheckScore()
        {
            string colour = Character.ColourToString(player.Characters[0].Colour);
            Control[] control = Controls.Find($"lblScore{colour}", true);
            switch (colour)
            {
                case "Green":
                    ((Label)control[0]).Text = $"Grön: {score}";
                    break;
                case "Yellow":
                    ((Label)control[0]).Text = $"Gul: {score}";
                    break;
                case "Red":
                    ((Label)control[0]).Text = $"Röd: {score}";
                    break;
                case "Blue":
                    ((Label)control[0]).Text = $"Blå: {score}";
                    break;
            }

            if (score == 4)
                Server.HasWon(server, player, serverType != 0);
        }
    }
}
