using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Deployment.Application;
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
        private static List<PictureBox> characters = new List<PictureBox>(26);
        private static PictureBox selectedCharacter;
        private static bool appClosingSelf = false;

        public FrmGame()
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

        private void btnLeave_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.No;

            if(serverType == FrmMenu.ServerType.HOSTING)
                result = MessageBox.Show("Är du säker på att du vill lämna? Alla andra anslutna till servern kommer också att tvingas lämna.", "Är du säker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            else
                result = MessageBox.Show("Är du säker på att du vill lämna?", "Är du säker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Disconnect the player
            if (result == DialogResult.Yes)
            {
                appClosingSelf = true;

                // If you choose to leave, you automatically lose the game
                LoseGame();

                if (serverType == FrmMenu.ServerType.HOSTING)
                {
                    // Make sure everyone goes back to lobby
                    Server.LeaveToLobby(server);
                    ExitToLobby();
                }
                else
                {
                    player = null;

                    // If it is currently your turn, make sure to change it before leaving
                    if (Game.CurrentTurn.Equals(player))
                        Server.RequestChangeOfTurn(server);

                    // Make sure everyone goes back to lobby
                    Server.Disconnect(server, player, false);
                    ExitToLobby(false);
                }
            }
        }

        private void btnLeaveToDesktop_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.No;

            if (serverType == FrmMenu.ServerType.HOSTING)
                result = MessageBox.Show("Är du säker på att du vill lämna? Alla andra anslutna till servern kommer också att tvingas lämna.", "Är du säker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            else
                result = MessageBox.Show("Är du säker på att du vill lämna?", "Är du säker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Disconnect the player and exit to desktop
            if (result == DialogResult.Yes)
            {
                appClosingSelf = true;

                // If you choose to leave, you automatically lose the game
                LoseGame();

                if (serverType == FrmMenu.ServerType.HOSTING)
                {
                    // Stop the server and exit the application, also send each connected players back to the lobby
                    Server.StopDuringGame(server);
                    Application.Exit();
                }
                else
                {
                    // If it is currently your turn, make sure to change it before leaving
                    if (Game.CurrentTurn.Equals(player))
                        Server.RequestChangeOfTurn(server);

                    // Disconnect from the server and exit the application
                    Server.Disconnect(server, player, false);
                    Application.Exit();
                }
            }
        }

        /// <summary>
        /// If a player chooses to close the form by pressing the X button, he is effectively
        /// doing the same as pressing the btnLeaveToDesktop button
        /// </summary>
        private void FrmGame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && !appClosingSelf)
            {
                DialogResult result = DialogResult.No;

                if (serverType == FrmMenu.ServerType.HOSTING)
                    result = MessageBox.Show("Är du säker på att du vill lämna? Alla andra anslutna till servern kommer också att tvingas lämna.", "Är du säker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                else
                    result = MessageBox.Show("Är du säker på att du vill lämna?", "Är du säker?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // If you choose to leave, you automatically lose the game
                    LoseGame();

                    if (serverType == FrmMenu.ServerType.HOSTING)
                    {
                        // Stop the server and exit the application, also send each connected players back to the lobby
                        Server.StopDuringGame(server);
                        Application.Exit();
                    }
                    else
                    {
                        // Disconnect from the server and exit the application
                        Server.Disconnect(server, player, false);
                        Application.Exit();
                    }
                }
                else e.Cancel = true;
            }
            else appClosingSelf = false;
        }

        private void PictureBoxPath_Click(object sender, EventArgs e)
        {
            Square path = Game.Squares[int.Parse(((PictureBox)sender).Name.Substring(3))];
            // Make sure the user pressed a bordered path
            if (path.Border == Square.SquareBorder.BORDER)
            {
                // Get the Character object
                string number = selectedCharacter.Name.Replace("pbxChar", "").Replace(Character.ColourToString(player.Characters[0].Colour), "");
                int index = int.Parse(number);
                Character character = player.Characters[index];

                // Move the character both on this client and all others
                Server.MoveCharacter(server, path, character, ((PictureBox)sender).Location, selectedCharacter, serverType != 0);
                Movement.MoveCharacter(path, character, ((PictureBox)sender).Location, selectedCharacter);

                // If the path the character was moved to was the goal path, add one to the player's score
                // and remove the character from the game
                if(path.Path == Square.PathType.GOAL)
                {
                    character.State = Character.CharacterState.WON;
                    Controls.Remove(selectedCharacter);

                    // Update the player's score
                    string colour = Character.ColourToString(player.Characters[0].Colour);
                    switch (colour)
                    {
                        case "Green":
                            int greenScore = int.Parse(lblScoreGreen.Text.Substring(6)) + 1;
                            lblScoreGreen.Text = $"Grön: {greenScore}";
                            break;
                        case "Yellow":
                            int yellowScore = int.Parse(lblScoreYellow.Text.Substring(5)) + 1;
                            lblScoreYellow.Text = $"Gul: {yellowScore}";
                            break;
                        case "Red":
                            int redScore = int.Parse(lblScoreRed.Text.Substring(5)) + 1;
                            lblScoreRed.Text = $"Röd: {redScore}";
                            break;
                        case "Blue":
                            int blueScore = int.Parse(lblScoreBlue.Text.Substring(5)) + 1;
                            lblScoreBlue.Text = $"Blå: {blueScore}";
                            break;
                    }

                    // Check if someone has won
                    CheckScore();
                }

                selectedCharacter = null;

                // The user has moved, change the turn unless the user may move again
                if (!reThrowAllowed && Game.CurrentTurn.Equals(player))
                {
                    if (serverType == FrmMenu.ServerType.HOSTING)
                    {
                        // Change the turn
                        Game.ChangeTurn(players);
                        TurnChanged();

                        // Notify the other clients of the new turn
                        Server.ChangeTurn(server, Game.CurrentTurn, serverType != 0);
                    }
                    else
                    {
                        // Request the server to change the turn
                        // The server has to change the turn because not everyone's
                        // list of Players is in the same order
                        Server.RequestChangeOfTurn(server);
                    }
                }

                UpdateBoard();

                diceThrown = false;
            }
        }

        private void pbxCharacter_Click(object sender, EventArgs e)
        {
            PictureBox pbx = (PictureBox)sender;

            // Make sure you're pressing one of your own characters
            if (pbx.Name.Substring(8).Equals(Character.ColourToString(player.Characters[0].Colour)))
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

                    // First remove any other borders there may be
                    foreach (Square s in Game.Squares) s.Border = Square.SquareBorder.BORDERLESS;

                    // Check if and where a player may move
                    Movement.DrawMovementLine(diceResult, character);

                    UpdateBoard();
                }
            }
        }


        private void pbxDice_Click(object sender, EventArgs e)
        {
            // Only throw the dice if it is the player's turn
            if (Game.CurrentTurn.Equals(player) && (!diceThrown || reThrowAllowed))
            {
                // Get a result
                diceResult = new Random().Next(1, 7);
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

                        diceThrown = false;

                        if(serverType == FrmMenu.ServerType.HOSTING)
                        {

                            // Change the turn
                            Game.ChangeTurn(players);
                            TurnChanged();

                            // Notify the other clients of the new turn
                            Server.ChangeTurn(server, Game.CurrentTurn, serverType != 0);
                        }
                        else
                        {
                            // Request the server to change the turn
                            // The server has to change the turn because not everyone's
                            // list of Players is in the same order
                            Server.RequestChangeOfTurn(server);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This methods handles the messages recieved from the server
        /// </summary>
        public void HandleMessageRecievedByServer(string message)
        {
            // Player is null when the player leaves the game by their own choosing
            if(player != null)
            {
                string msgType = $"{message[0]}{message[1]}{message[2]}";
                string[] data = message.Split('|');
                switch (msgType)
                {
                    case "PLD": // Player disconnected

                        // Loop through the player list and remove the disconnected player
                        string colourPLD = "";
                        for (int i = 0; i < players.Count; i++)
                        {
                            if (players[i].Name == data[1])
                            {
                                colourPLD = Character.ColourToString(players[i].Characters[0].Colour);
                                players.RemoveAt(i);
                                break;
                            }
                        }

                        // Remove the player's characters from the board
                        foreach (PictureBox pbxChar in characters)
                        {
                            // Find the colour of the character
                            string colour = pbxChar.Name.Replace("pbxChar", "").Substring(1);
                            if (colour.Equals(colourPLD))
                                Controls.Remove(pbxChar);
                        }

                        // See if you're the last player online, if you are, you win
                        if (players.Count == 1)
                        {
                            // Increase the player's score to four
                            Label playerScore = (Label)Controls.Find($"lblScore{Character.ColourToString(player.Characters[0].Colour)}", true)[0];
                            playerScore.Text = playerScore.Text.Length == 7 ? $"{playerScore.Text.Substring(0, 6)}4" : $"{playerScore.Text.Substring(0, 5)}4";

                            // Call the CheckScore() method to find that this player is the winner
                            CheckScore();
                        }

                        break;
                    case "LTL": // A message to all clients telling them to leave the game back to lobby
                        MessageBox.Show("Värden har avslutat spelet. Du kommer nu bli tillbakaskickad till spelmenyn.", "Spelet är avslutat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ExitToLobby();
                        break;
                    case "SDG": // A message to all clients informing them that the host has stopped the server
                        // See if you're the last player online, if you are, you win
                        if (players.Count == 1)
                        {
                            int gamesWon = 0;
                            int gamesLost = 0;
                            FileHandler.ReadUserData(player.Name, ref gamesWon, ref gamesLost);
                            FileHandler.SaveUserData(player.Name, ++gamesWon, gamesLost);
                        }

                        MessageBox.Show("Värden har stoppat servern. Du kommer nu bli tillbakaskickad till spelmenyn.", "Spelet är avslutat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ExitToLobby(false);
                        break;
                    case "MVC": // A character has been moved

                        // Find the colour sent over
                        if (Character.TryParseColour(data[3], out Color colourMVC))
                        {
                            // Convert the data into the objects they are representing
                            Square path = Game.Squares[int.Parse(data[1])];
                            Point pathLocation = new Point(int.Parse(data[4]), int.Parse(data[5]));
                            PictureBox pbxCharacter = (PictureBox)Controls.Find(data[6], true)[0];
                            Character character = null;

                            // Find the character
                            if (int.Parse(data[2]) == -1) // This is true if the character is at home
                            {
                                foreach (Player p in players)
                                {
                                    if (p.Characters[0].Colour.Equals(colourMVC))
                                    {
                                        foreach (Character c in p.Characters)
                                        {
                                            if (c.State == Character.CharacterState.HOME)
                                            {
                                                character = c;
                                                break;
                                            }
                                        }

                                        if (character != null) break;
                                    }
                                }
                            }
                            else // Since the character was not at home, one can find it by finding where he is currently standing
                            {
                                character = Game.Squares[int.Parse(data[2])].Character;
                            }

                            // Call the MoveCharacter() method with the decoded objects above
                            Movement.MoveCharacter(path, character, pathLocation, pbxCharacter);

                            // If the path the character was moved to was the goal path, 
                            // add one to the respective player's score and remove the 
                            // character from the game
                            if (path.Path == Square.PathType.GOAL)
                            {
                                switch (Character.ColourToString(character.Colour))
                                {
                                    case "Green":
                                        int greenScore = int.Parse(lblScoreGreen.Text.Substring(6)) + 1;
                                        lblScoreGreen.Text = $"Grön: {greenScore}";
                                        break;
                                    case "Yellow":
                                        int yellowScore = int.Parse(lblScoreYellow.Text.Substring(5)) + 1;
                                        lblScoreYellow.Text = $"Gul: {yellowScore}";
                                        break;
                                    case "Red":
                                        int redScore = int.Parse(lblScoreRed.Text.Substring(5)) + 1;
                                        lblScoreRed.Text = $"Röd: {redScore}";
                                        break;
                                    case "Blue":
                                        int blueScore = int.Parse(lblScoreBlue.Text.Substring(5)) + 1;
                                        lblScoreBlue.Text = $"Blå: {blueScore}";
                                        break;
                                }

                                character.State = Character.CharacterState.WON;
                                pbxCharacter.Enabled = false;
                                pbxCharacter.Visible = false;

                                CheckScore();
                            }
                        }

                        break;
                    case "HAW": // A player has won
                        SomeoneWon(data[1]);
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
                    case "CTR": // A request to change the turn has been sent to the server

                        Game.ChangeTurn(players);
                        TurnChanged();

                        // Notify the other clients of the new turn
                        Server.ChangeTurn(server, Game.CurrentTurn, serverType != 0);

                        break;
                    case "CHT": // The turn has been changed, only clients recieve this message

                        foreach (Player p in players)
                        {
                            if (p.Name.Equals(data[1]))
                            {
                                Game.CurrentTurn = p;
                                break;
                            }
                        }

                        TurnChanged();

                        break;
                }
            }
        }

        private void FrmGame_Shown(object sender, EventArgs e)
        {
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

            Movement.Form = this;
            Movement.Characters = characters;
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
                PictureBox[] chars = Character.CreateCharacters(p.Characters[0].Colour);
                foreach (PictureBox pbx in chars)
                {
                    pbx.Click += pbxCharacter_Click;
                    Controls.Add(pbx);
                    Controls.SetChildIndex(pbx, 0);
                    characters.Add(pbx);
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
        /// Check the players' score to determine if anyone has won
        /// </summary>
        private void CheckScore()
        {
            // Get the colour of the player
            Color colour = player.Characters[0].Colour;

            // Prepare a variable to hold the colour of the winner
            Color winner = Color.White;

            // Fetch the different scores
            int greenScore = int.Parse(lblScoreGreen.Text.Substring(6));
            int yellowScore = int.Parse(lblScoreYellow.Text.Substring(5));
            int redScore = int.Parse(lblScoreRed.Text.Substring(5));
            int blueScore = int.Parse(lblScoreBlue.Text.Substring(5));

            // Keep track of how many games the player has won and lost
            int gamesWon = 0;
            int gamesLost = 0;

            if (greenScore == 4) winner = Color.Green;
            else if (yellowScore == 4) winner = Color.Yellow;
            else if (redScore == 4) winner = Color.Red;
            else if (blueScore == 4) winner = Color.Blue;

            // Read how many games the user has won and lost
            FileHandler.ReadUserData(player.Name, ref gamesWon, ref gamesLost);

            // Increment the games won by one if the player won, otherwise increment the games lost by one
            if (colour == winner)
            {
                FileHandler.SaveUserData(player.Name, ++gamesWon, gamesLost);
                Server.HasWon(server, winner, serverType != 0);

                SomeoneWon(Character.ColourToString(winner));
            }
            else if (winner != Color.White) // If someone else has won, the winner colour IS NOT white
            {
                FileHandler.SaveUserData(player.Name, gamesWon, ++gamesLost);
                Server.HasWon(server, winner, serverType != 0);

                SomeoneWon(Character.ColourToString(winner));
            }
        }

        /// <summary>
        /// The method that handles all the logic when a player wins
        /// </summary>
        private void SomeoneWon(string data)
        {
            // Find the winner amongst the list of players
            string colour = data;
            Player winner = null;

            foreach (Player p in players)
            {
                if (Character.ColourToString(p.Characters[0].Colour).Equals(colour))
                {
                    winner = p;
                    break;
                }
            }

            // Winner could end up being null if a player leaves and the message that someone has won
            // is recieved by the player who left quicker than he can disconnect from the server.
            // The reason the player who's trying to disconnect cannot find a winner is because the points
            // give to someone who wins by default to being the last player online are ONLY given client-side.
            if(winner != null)
            {
                // Display to everyone who has won
                if (winner.Name.Equals(player.Name))
                    MessageBox.Show("Grattis, du vann spelet!\nSnyggt gjort!", "Du är en vinnare!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                    MessageBox.Show($"Tyvärr vann du inte...\nDet gjorde {winner.Name}!", "Det var ett bra försök!", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ExitToLobby();
            }
        }

        /// <summary>
        /// Exit back to lobby
        /// </summary>
        /// <param name="connected">True by default, informs the Menu form if the player is still connected or not</param>
        private void ExitToLobby(bool connected = true)
        {
            // Exit to menu
            this.Hide();
            FrmMenu frmMenu = new FrmMenu();

            // Update the form
            frmMenu.UpdateFormOnGameEnd(connected);

            // Show the menu
            frmMenu.ShowDialog();
            appClosingSelf = true;
            this.Close();
        }

        /// <summary>
        /// This method makes the player automatically lose the game
        /// </summary>
        private void LoseGame()
        {
            int gamesWon = 0;
            int gamesLost = 0;
            FileHandler.SaveUserData(player.Name, gamesWon, ++gamesLost);
        }
    }
}
