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
        /// Constructor for the <see cref="Square"/> class
        /// </summary>
        /// <param name="colour">The <see cref="Color"/> of the <see cref="Square"/>, if black, it's the middle <see cref="Square"/></param>
        /// <param name="index">The index of the <see cref="Square"/></param>
        /// <param name="path">The <see cref="Square"/>'s <see cref="PathType"/></param>
        /// <exception cref="InvalidColourException"/>
        /// <exception cref="System.IndexOutOfRangeException"/>
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

        public override string ToString()
        {
            return $"Colour: {Character.ColourToString(Colour)} | Pos: {Position} | State: {State} | BorderState: {Border} | PathType: {Path}";
        }

        public Color Colour { get; set; }
        public Character Character { get; set; }
        public SquareState State { get; set; }
        public SquareBorder Border { get; set; }
        public PathType Path { get; set; }
        public int Position { get; set; }
    }
}