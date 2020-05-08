using System.Drawing;

namespace FiaMedKnuff
{
    public class Character
    {
        /// <summary>
        /// Constructor for the Character class
        /// </summary>
        /// <param name="player">The player who is linked to this character</param>
        /// <param name="color">The colour of this character</param>
        public Character(Player player, Color colour)
        {
            this.Player = player;
            if (colour != Color.Green && colour != Color.Red && colour != Color.Yellow && colour != Color.Blue)
                throw new InvalidColourException();
            this.Colour = colour;
        }

        public override string ToString()
        {
            return $"Owner: {Player} | Colour: {Colour}";
        }

        public Player Player { get; set; }
        public Color Colour { get; set; }
        public int Position { get; set; }
    }
}