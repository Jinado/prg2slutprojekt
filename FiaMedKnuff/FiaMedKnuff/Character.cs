using FiaMedKnuff.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public class Character
    {
        public enum CharacterState { HOME, OUTSIDE, WON };

        // The ARGB colour value of the colours used in the form
        public static readonly Color GREEN = Color.FromArgb(0, 166, 81);
        public static readonly Color YELLOW = Color.FromArgb(255, 204, 1);
        public static readonly Color RED = Color.FromArgb(252, 8, 37);
        public static readonly Color BLUE = Color.FromArgb(54, 68, 206);
        public static readonly Color BLACK = Color.FromArgb(0, 0, 0);

        /// <summary>
        /// Constructor for the Character class
        /// </summary>
        /// <param name="colour">The colour of this character</param>
        /// <exception cref="InvalidColourException"></exception>
        public Character(Color colour)
        {
            if (colour != Color.Green && colour != Color.Red && colour != Color.Yellow && colour != Color.Blue)
                throw new InvalidColourException();
            this.Colour = colour;
            this.State = CharacterState.HOME;
            this.Position = -1;
        }

        /// <summary>
        /// Create a list of characters of a specific colour
        /// </summary>
        /// <param name="colour">The characters' colour</param>
        /// <returns>A list of characters</returns>
        public static List<Character> Assign(Color colour)
        {
            List<Character> chars = new List<Character>(4);
            for(int i = 0; i < 4; i++)
                chars.Add(new Character(colour));

            return chars;
        }

        /// <summary>
        /// Parses a string gotten from Color.ToString() and returns a colour
        /// </summary>
        /// <param name="str">The string to parse</param>
        /// <param name="colour">The colour parsed from the string</param>
        /// <returns>True if the colour is correct</returns>
        public static bool TryParseColour(string str, out Color colour)
        {
            if (str.Equals("Color [Yellow]"))
            {
                colour = Color.Yellow;
                return true;
            }
            else if(str.Equals("Color [Red]"))
            {
                colour = Color.Red;
                return true;
            }       
            else if (str.Equals("Color [Blue]"))
            {
                colour = Color.Blue;
                return true; 
            }              
            else if (str.Equals("Color [Green]"))
            {
                colour = Color.Green;
                return true;
            }

            colour = Color.White;
            return false;
        }

        public static string ColourToString(Color colour)
        {
            return colour.ToString().Split('[')[1].Trim(']');
        }

        public static PictureBox[] CreateCharacters(Color colour)
        {
            PictureBox[] pictureBoxes = new PictureBox[4];
            for(int i = 0; i < 4; i++)
            {
                PictureBox pbx = new PictureBox();
                pbx.Size = new Size(64, 60);
                string colourAsString = ColourToString(colour);
                pbx.Name = $"pbxChar{i}{colourAsString}";
                pbx.SizeMode = PictureBoxSizeMode.Zoom;
                pbx.Cursor = Cursors.Hand;

                // The tag will contain the colour of the character
                pbx.Tag = Character.ColourToString(colour);

                switch (colourAsString)
                {
                    case "Green":
                        pbx.Image = Resources.char_green;
                        pbx.BackColor = GREEN;
                        if (i == 0)
                            pbx.Location = new Point(54, 590);
                        else if (i == 1)
                            pbx.Location = new Point(133, 524);
                        else if (i == 2)
                            pbx.Location = new Point(133, 590);
                        else
                            pbx.Location = new Point(54, 524);
                        break;
                    case "Yellow":
                        pbx.Image = Resources.char_yellow;
                        pbx.BackColor = YELLOW;
                        if (i == 0)
                            pbx.Location = new Point(54, 82);
                        else if (i == 1)
                            pbx.Location = new Point(133, 148);
                        else if (i == 2)
                            pbx.Location = new Point(54, 148);
                        else
                            pbx.Location = new Point(133, 82);
                        break;
                    case "Red":
                        pbx.Image = Resources.char_red;
                        pbx.BackColor = RED;
                        if (i == 0)
                            pbx.Location = new Point(659, 82);
                        else if (i == 1)
                            pbx.Location = new Point(580, 148);
                        else if (i == 2)
                            pbx.Location = new Point(580, 82);
                        else
                            pbx.Location = new Point(659, 148);
                        break;
                    case "Blue":
                        pbx.Image = Resources.char_blue;
                        pbx.BackColor = BLUE;
                        if (i == 0)
                            pbx.Location = new Point(659, 590);
                        else if (i == 1)
                            pbx.Location = new Point(580, 524);
                        else if (i == 2)
                            pbx.Location = new Point(659, 524);
                        else
                            pbx.Location = new Point(580, 590);
                        break;
                }

                pictureBoxes[i] = pbx;
            }

            return pictureBoxes;
        }

        public override string ToString()
        {
            return $"Colour: {Colour}";
        }

        public Color Colour { get; set; }
        public int Position { get; set; }
        public CharacterState State { get; set; }
    }
}