using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    static public class Dice
    {
        /// <summary>
        /// Rolls the dice
        /// </summary>
        /// <param name="reRoll">Is true if the player rolls a six, since he may then re-roll the dice</param>
        /// <returns>The result of the dice roll</returns>
        static public byte RollDice(out bool reRoll)
        {
            reRoll = false;
            byte result = (byte)(new Random().Next(0, 7));
            if (result == 6) reRoll = true;

            return result;
        }
    }
}
