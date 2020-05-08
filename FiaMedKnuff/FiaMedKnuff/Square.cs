using System.Drawing;

namespace FiaMedKnuff
{
    public class Square
    {
        public enum SquareState { OCCUPIED, FREE };
        
        /// <summary>
        /// Constructor for the Square class
        /// </summary>
        /// <param name="colour">The colour of the Square, if black, it's the middle Square</param>
        public Square(Color colour)
        {
            this.State = SquareState.FREE;
            if (colour != Color.Green && colour != Color.Red && colour != Color.Yellow && colour != Color.Blue || colour != Color.Black)
                throw new InvalidColourException("Invalid colour, it must be green, red, yellow, blue or black");
            this.Colour = colour;
        }

        public Color Colour { get; set; }
        public Character Character { get; set; }
        public SquareState State { get; set; }
    }
}