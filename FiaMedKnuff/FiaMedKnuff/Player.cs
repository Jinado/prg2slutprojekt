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
        }

        /// <summary>
        /// Constructor for the Player class
        /// </summary>
        /// <param name="name">The name of the player</param>
        /// <param name="state">The ready state of a player</param>
        public Player(string name, PlayerState state)
        {
            this.Name = name;
            this.State = state;
        }

        public override string ToString()
        {
            return $"Name: {Name} | ReadyState: {State}";
        }

        public string Name { get; set; }
        public List<Character> Characters { get; set; }
        public PlayerState State { get; set; }
    }
}