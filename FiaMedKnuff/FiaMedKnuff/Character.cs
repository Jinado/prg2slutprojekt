using System.Collections.Generic;
using System.Drawing;

namespace FiaMedKnuff
{
    public class Character
    {
        public enum CharacterState { HOME, OUTSIDE };

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

        public override string ToString()
        {
            return $"Colour: {Colour}";
        }

        public Player Player { get; set; }
        public Color Colour { get; set; }
        public int Position { get; set; }
        public CharacterState State { get; set; }
    }
}