using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    abstract public class Game
    {
        // public enum GameState { } ???

        static public List<Player> players = new List<Player>();
        static public List<Square> squares = new List<Square>();
        static public byte maxPlayers = 4;
        static public byte ammountOfPlayers;
        static public Player currentTurn;

        /// <summary>
        /// Check if everyone is ready to start the game
        /// </summary>
        /// <returns>True if everyone is ready</returns>
        public bool ReadyToStart()
        {
            foreach(Player p in players)
            {
                if (p.State == Player.PlayerState.NOT_READY)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Initialize a game
        /// </summary>
        public void InitGame(List<Player> players)
        {
            // Populate player list
            Game.players = players;

            // Choose random player to begin
            currentTurn = players[new Random().Next(0, maxPlayers)];
        }

        /// <summary>
        /// Changes the current turn so that it's the next player's turn
        /// </summary>
        /// <returns>The player whose turn it now is</returns>
        public Player ChangeTurn()
        {
            int index = -1;
            for (int i = 0; i < players.Count; i++)
            {
                index = players[i].Equals(currentTurn) ? i : -1;
                if (index != -1) break;
            }

            return index == (players.Count - 1) ? players[0] : players[index + 1];
        }

        public override string ToString()
        {
            return $"Connected players: {ammountOfPlayers}, max players: {maxPlayers}, current turn: {currentTurn}";
        }
    }
}
