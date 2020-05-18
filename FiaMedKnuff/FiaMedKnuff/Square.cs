using System.Drawing;
using System.IO;

namespace FiaMedKnuff
{
    public class Square
    {
        public enum SquareState { OCCUPIED, FREE };
        public enum SquareBorder { BORDERLESS, BORDER };
        public enum PathType { STANDARD, FINAL, GOAL };
        
        /// <summary>
        /// Constructor for the Square class
        /// </summary>
        /// <param name="colour">The colour of the Square, if black, it's the middle Square</param>
        /// <param name="index">The index of the Square</param>
        /// <param name="path">The type of path the square is</param>
        /// <exception cref="InvalidColourException"></exception>
        /// <exception cref="System.IndexOutOfRangeException"></exception>
        public Square(Color colour, int index, PathType path = PathType.STANDARD)
        {
            this.State = SquareState.FREE;
            if (colour != Color.Green && colour != Color.Red && colour != Color.Yellow && colour != Color.Blue && colour != Color.Black)
                throw new InvalidColourException("Invalid colour, it must be green, red, yellow, blue or black");

            if (index < 0 || index >= 57)
                throw new System.IndexOutOfRangeException("The index may not be larger than 56 nor smaller than 0");

            this.Colour = colour;
            this.Position = index;
            this.Border = SquareBorder.BORDERLESS;
            this.Path = path;
        }

        public Color Colour { get; set; }
        public Character Character { get; set; }
        public SquareState State { get; set; }
        public SquareBorder Border { get; set; }
        public PathType Path { get; set; }
        public int Position { get; set; }
    }
}