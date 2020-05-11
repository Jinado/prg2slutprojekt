using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    static public class Movement
    {
        // The index in the Game.Squares list representing the 
        // first square outside a character's home
        private const int GREEN_HOME = 0;
        private const int YELLOW_HOME = 10;
        private const int RED_HOME = 20;
        private const int BLUE_HOME = 30;

        /// <summary>
        /// Draws a border around the square a character may move to
        /// </summary>
        /// <param name="diceResult">The result of a dice throw</param>
        /// <param name="character">The character the player wishes to move</param>
        static public void DrawMovementLine(byte diceResult, Character character)
        {
            int charPos = character.Position;
            Color charColour = character.Colour;
            Character.CharacterState charState = character.State;

            // Check if the character is home and if he may be moved outside
            if (charState == Character.CharacterState.HOME && (diceResult == 1 || diceResult == 6))
            {
                if(charColour == Color.Green)
                {
                    int index = diceResult == 1 ? GREEN_HOME : (GREEN_HOME + 6);
                    if (CheckIfPossible(Game.Squares[index], character))
                        Game.Squares[index].Border = Square.SquareBorder.BORDER;
                }
                else if (charColour == Color.Yellow)
                {
                    int index = diceResult == 1 ? YELLOW_HOME : (YELLOW_HOME + 6);
                    if (CheckIfPossible(Game.Squares[index], character))
                        Game.Squares[index].Border = Square.SquareBorder.BORDER;
                }
                else if (charColour == Color.Red)
                {
                    int index = diceResult == 1 ? RED_HOME : (RED_HOME + 6);
                    if (CheckIfPossible(Game.Squares[index], character))
                        Game.Squares[index].Border = Square.SquareBorder.BORDER;
                }
                else if (charColour == Color.Blue)
                {
                    int index = diceResult == 1 ? BLUE_HOME : (BLUE_HOME + 6);
                    if (CheckIfPossible(Game.Squares[index], character))
                        Game.Squares[index].Border = Square.SquareBorder.BORDER;
                }
            }
            else // The character is not home, see where he may move to
            {
                if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                    Game.Squares[charPos + diceResult].Border = Square.SquareBorder.BORDER;
            }
        }

        /// <summary>
        /// Checks if it is possible for a specified character to move to a specified square
        /// </summary>
        /// <param name="square">The square to move to</param>
        /// <param name="character">The character to move</param>
        /// <returns>True if the character may move to the square</returns>
        static private bool CheckIfPossible(Square square, Character character)
        {
            // Check if there is a character on the square already
            if(square.State == Square.SquareState.OCCUPIED)
            {
                Color occupantColour = square.Character.Colour;

                // See if the character to be moved is of the same colour as the occupant
                if (occupantColour == character.Colour)
                {
                    return false;
                }
                else
                {
                    // See if the occupant is on a safe square
                    if(occupantColour == square.Colour)
                        return false;
                    else
                        return true;
                }
            }

            // The square is free
            return true;
        }

        /// <summary>
        /// Moves a specified character to a specified square
        /// </summary>
        /// <param name="square">The square to move to</param>
        /// <param name="character">The character to move</param>
        static public void MoveCharacter(Square square, Character character)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pushes an opponent
        /// </summary>
        /// <param name="opponent">The opponent to push</param>
        static private void PushOpponent(Character opponent)
        {
            throw new NotImplementedException();
        }
    }
}
