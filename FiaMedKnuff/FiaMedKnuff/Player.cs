using System.Collections.Generic;

namespace FiaMedKnuff
{
    public class Player
    {
        public enum PlayerState { NOT_READY, READY };

        /// <summary>
        /// Constructor for the Player class
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="characters">The characters the player controls</param>
        /// <param name="state">The ready state of a player</param>
        public Player(string name, List<Character> characters, PlayerState state)
        {
            this.Name = name;
            this.Characters = characters;
            this.State = state;
            this.PlayersTurn = false;
        }

        public override string ToString()
        {
            return $"{Name} : {State} : {PlayersTurn}";
        }

        public string Name { get; set; }
        public List<Character> Characters { get; set; }
        public PlayerState State { get; set; }
        public bool PlayersTurn { get; set; }
    }
}