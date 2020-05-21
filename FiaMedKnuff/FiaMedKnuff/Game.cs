using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    abstract public class Game
    {

        /// <summary>
        /// Initialize a game
        /// </summary>
        /// <param name="players">A list of connected <see cref="Player">players</see></param>
        /// <param name="maxPlayers">The amount of max <see cref="Player">players</see> connected to the server at once</param>
        public static void Initialize(List<Player> players, int maxPlayers)
        {
            // Populate the Squares list
            // Below is the full path around the map
            Squares.Add(new Square(Color.Green, 0));
            Squares.Add(new Square(Color.Red, 1));
            Squares.Add(new Square(Color.Yellow, 2));
            Squares.Add(new Square(Color.Blue, 3));
            Squares.Add(new Square(Color.Red, 4));
            Squares.Add(new Square(Color.Green, 5));
            Squares.Add(new Square(Color.Yellow, 6));
            Squares.Add(new Square(Color.Blue, 7));
            Squares.Add(new Square(Color.Green, 8));
            Squares.Add(new Square(Color.Yellow, 9));
            Squares.Add(new Square(Color.Yellow, 10));
            Squares.Add(new Square(Color.Blue, 11));
            Squares.Add(new Square(Color.Green, 12));
            Squares.Add(new Square(Color.Red, 13));
            Squares.Add(new Square(Color.Blue, 14));
            Squares.Add(new Square(Color.Yellow, 15));
            Squares.Add(new Square(Color.Green, 16));
            Squares.Add(new Square(Color.Red, 17));
            Squares.Add(new Square(Color.Yellow, 18));
            Squares.Add(new Square(Color.Red, 19));
            Squares.Add(new Square(Color.Red, 20));
            Squares.Add(new Square(Color.Yellow, 21));
            Squares.Add(new Square(Color.Blue, 22));
            Squares.Add(new Square(Color.Green, 23));
            Squares.Add(new Square(Color.Yellow, 24));
            Squares.Add(new Square(Color.Red, 25));
            Squares.Add(new Square(Color.Blue, 26)); 
            Squares.Add(new Square(Color.Green, 27));
            Squares.Add(new Square(Color.Red, 28));
            Squares.Add(new Square(Color.Blue, 29));
            Squares.Add(new Square(Color.Blue, 30));
            Squares.Add(new Square(Color.Green, 31)); 
            Squares.Add(new Square(Color.Red, 32)); 
            Squares.Add(new Square(Color.Yellow, 33)); 
            Squares.Add(new Square(Color.Green, 34)); 
            Squares.Add(new Square(Color.Blue, 35)); 
            Squares.Add(new Square(Color.Red, 36)); 
            Squares.Add(new Square(Color.Yellow, 37)); 
            Squares.Add(new Square(Color.Blue, 38)); 
            Squares.Add(new Square(Color.Green, 39));

            // Below is the final stretches before the players reach the black dot
            Squares.Add(new Square(Color.Green, 40, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Green, 41, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Green, 42, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Green, 43, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Yellow, 44, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Yellow, 45, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Yellow, 46, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Yellow, 47, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Red, 48, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Red, 49, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Red, 50, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Red, 51, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Blue, 52, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Blue, 53, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Blue, 54, Square.PathType.FINAL));
            Squares.Add(new Square(Color.Blue, 55, Square.PathType.FINAL));

            // Below is the black dot in the center. The goal.
            Squares.Add(new Square(Color.Black, 56, Square.PathType.GOAL));

            // Max amount of players
            MaxPlayers = maxPlayers;

            // Choose random player to begin
            CurrentTurn = players[new Random().Next(0, maxPlayers)];
        }

        /// <summary>
        /// Changes the current turn so that it's the next <see cref="Player"/>'s turn
        /// </summary>
        /// <param name="players">A list of <see cref="Player">players</see> to use when changing the turn</param>
        public static void ChangeTurn(List<Player> players)
        {
            if(players.Count != 0)
            {
                int index = -1;
                for (int i = 0; i < players.Count; i++)
                {
                    index = players[i].Equals(CurrentTurn) ? i : -1;
                    if (index != -1) break;
                }

                CurrentTurn = index == (players.Count - 1) ? players[0] : players[index + 1];
            }
        }

        public override string ToString()
        {
            return $"Max players: {MaxPlayers} | Current turn: {CurrentTurn}";
        }

        static public List<Square> Squares { get; } = new List<Square>(56);
        static public int MaxPlayers { get; set; }
        static public Player CurrentTurn { get; set; }
    }
}
