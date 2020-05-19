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
        // public enum GameState { } ???

        static private List<Square> squares = new List<Square>(56);
        static private int maxPlayers = 4;
        static private int ammountOfPlayers;
        static private Player currentTurn;

        /// <summary>
        /// Initialize a game
        /// </summary>
        /// <param name="players">A list of connected players</param>
        /// <param name="maxPlayers">The amount of max players</param>
        public static void Initialize(List<Player> players, int maxPlayers)
        {
            // Populate the Game.Squares list
            // Below is the full path around the map
            Game.squares.Add(new Square(Color.Green, 0));
            Game.squares.Add(new Square(Color.Red, 1));
            Game.squares.Add(new Square(Color.Yellow, 2));
            Game.squares.Add(new Square(Color.Blue, 3));
            Game.squares.Add(new Square(Color.Red, 4));
            Game.squares.Add(new Square(Color.Green, 5));
            Game.squares.Add(new Square(Color.Yellow, 6));
            Game.squares.Add(new Square(Color.Blue, 7));
            Game.squares.Add(new Square(Color.Green, 8));
            Game.squares.Add(new Square(Color.Yellow, 9));
            Game.squares.Add(new Square(Color.Yellow, 10));
            Game.squares.Add(new Square(Color.Blue, 11));
            Game.squares.Add(new Square(Color.Green, 12));
            Game.squares.Add(new Square(Color.Red, 13));
            Game.squares.Add(new Square(Color.Blue, 14));
            Game.squares.Add(new Square(Color.Yellow, 15));
            Game.squares.Add(new Square(Color.Green, 16));
            Game.squares.Add(new Square(Color.Red, 17));
            Game.squares.Add(new Square(Color.Yellow, 18));
            Game.squares.Add(new Square(Color.Red, 19));
            Game.squares.Add(new Square(Color.Red, 20));
            Game.squares.Add(new Square(Color.Yellow, 21));
            Game.squares.Add(new Square(Color.Blue, 22));
            Game.squares.Add(new Square(Color.Green, 23));
            Game.squares.Add(new Square(Color.Yellow, 24));
            Game.squares.Add(new Square(Color.Red, 25));
            Game.squares.Add(new Square(Color.Blue, 26)); 
            Game.squares.Add(new Square(Color.Green, 27));
            Game.squares.Add(new Square(Color.Red, 28));
            Game.squares.Add(new Square(Color.Blue, 29));
            Game.squares.Add(new Square(Color.Blue, 30));
            Game.squares.Add(new Square(Color.Green, 31)); 
            Game.squares.Add(new Square(Color.Red, 32)); 
            Game.squares.Add(new Square(Color.Yellow, 33)); 
            Game.squares.Add(new Square(Color.Green, 34)); 
            Game.squares.Add(new Square(Color.Blue, 35)); 
            Game.squares.Add(new Square(Color.Red, 36)); 
            Game.squares.Add(new Square(Color.Yellow, 37)); 
            Game.squares.Add(new Square(Color.Blue, 38)); 
            Game.squares.Add(new Square(Color.Green, 39));

            // Below is the final stretches before the players reach the black dot
            Game.squares.Add(new Square(Color.Green, 40, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Green, 41, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Green, 42, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Green, 43, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Yellow, 44, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Yellow, 45, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Yellow, 46, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Yellow, 47, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Red, 48, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Red, 49, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Red, 50, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Red, 51, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Blue, 52, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Blue, 53, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Blue, 54, Square.PathType.FINAL));
            Game.squares.Add(new Square(Color.Blue, 55, Square.PathType.FINAL));

            // Below is the black dot in the center. The goal.
            Game.squares.Add(new Square(Color.Black, 56, Square.PathType.GOAL));

            // Max amount of players
            Game.maxPlayers = maxPlayers;

            // Choose random player to begin
            Game.currentTurn = players[new Random().Next(0, maxPlayers)];
        }

        /// <summary>
        /// Changes the current turn so that it's the next player's turn
        /// </summary>
        public static void ChangeTurn(List<Player> players)
        {
            int index = -1;
            for (int i = 0; i < players.Count; i++)
            {
                index = players[i].Equals(currentTurn) ? i : -1;
                if (index != -1) break;
            }

            Game.CurrentTurn = index == (players.Count - 1) ? players[0] : players[index + 1];
        }

        public override string ToString()
        {
            return $"Connected players: {ammountOfPlayers}, max players: {maxPlayers}, current turn: {currentTurn}";
        }

        // Getters and Setters for the private variables at the top of this class
        static public List<Square> Squares
        {
            get { return squares; }
        }
        static public int MaxPlayers
        {
            get { return maxPlayers; }
            set { maxPlayers = value; }
        }
        static public int AmmountOfPlayers
        {
            get { return ammountOfPlayers; }
            set { ammountOfPlayers = value; }
        }
        static public Player CurrentTurn
        {
            get { return currentTurn; }
            set { currentTurn = value; }
        }
    }
}
