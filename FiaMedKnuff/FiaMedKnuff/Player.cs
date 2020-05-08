using System.Collections.Generic;

namespace FiaMedKnuff
{
    public class Player
    {
        public enum PlayerState { NOT_READY, READY };

        public Player(string name, List<Character> characters, PlayerState state, bool playersTurn)
        {
            this.Name = name;
            this.Characters = characters;
            this.State = state;
            this.PlayersTurn = playersTurn;
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