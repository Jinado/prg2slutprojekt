using System.Collections.Generic;

namespace FiaMedKnuff
{
    public class Player
    {
        public enum PlayerState { NOT_READY, READY };

        /// <summary>
        /// Constructor for the <see cref="Player"/> class
        /// </summary>
        /// <param name="name">The name of the <see cref="Player"/></param>
        /// <param name="characters">The <see cref="Character">characters</see> the <see cref="Player"/> controls</param>
        /// <param name="state">The <see cref="Player"/>'s <see cref="PlayerState"/>. This keeps track of if the <see cref="Player"/> is ready or not</param>
        public Player(string name, List<Character> characters, PlayerState state)
        {
            this.Name = name;
            this.Characters = characters;
            this.State = state;
        }

        /// <summary>
        /// Constructor for the <see cref="Player"/> class
        /// </summary>
        /// <param name="name">The name of the <see cref="Player"/></param>
        /// <param name="state">The ready state of a <see cref="Player"/></param>
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