using System.Drawing;

namespace FiaMedKnuff
{
    public class Square
    {
        public enum SquareState { OCCUPIED, FREE };
        public enum SquareBorder { BORDERLESS, BORDER };
        
        /// <summary>
        /// Constructor for the Square class
        /// </summary>
        /// <param name="colour">The colour of the Square, if black, it's the middle Square</param>
        /// <param name="index">The index of the Square</param>
        public Square(Color colour, int index)
        {
            this.State = SquareState.FREE;
            if (colour != Color.Green && colour != Color.Red && colour != Color.Yellow && colour != Color.Blue || colour != Color.Black)
                throw new InvalidColourException("Invalid colour, it must be green, red, yellow, blue or black");
            this.Colour = colour;
            if (index < 0 || index >= 57)
                throw new System.IndexOutOfRangeException("The index may not be larger than 56 nor smaller than 0");
            this.SquareIndex = index;
            this.Border = SquareBorder.BORDERLESS;
        }

        public Color Colour { get; set; }
        public Character Character { get; set; }
        public SquareState State { get; set; }
        public SquareBorder Border { get; set; }
        public int SquareIndex { get; set; }
    }
}