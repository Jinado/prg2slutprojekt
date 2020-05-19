using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiaMedKnuff
{
    public static class Movement
    {
        // Constant values to help orienting the map
        private const int GREEN_HOME = 0;
        private const int YELLOW_HOME = 10;
        private const int RED_HOME = 20;
        private const int BLUE_HOME = 30;
        private const int FIRST_GREEN_FINAL = 40;
        private const int FIRST_YELLOW_FINAL = 44;
        private const int FIRST_RED_FINAL = 48;
        private const int FIRST_BLUE_FINAL = 52;
        private const int GOAL = 56;

        /// <summary>
        /// Draws a border around the square a character may move to
        /// </summary>
        /// <param name="diceResult">The result of a dice throw</param>
        /// <param name="character">The character the player wishes to move</param>
        /// <returns>True if there was a possible move</returns>
        public static bool DrawMovementLine(int diceResult, Character character)
        {
            int charPos = character.Position;
            Color charColour = character.Colour;
            Character.CharacterState charState = character.State;

            // Check if the character is home and if he may be moved outside
            if (charState == Character.CharacterState.HOME)
            {
                if (diceResult == 1 || diceResult == 6)
                {
                    if (charColour.Equals(Color.Green))
                    {
                        int index = diceResult == 1 ? GREEN_HOME : (GREEN_HOME + 6);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = Square.SquareBorder.BORDER;
                            return true;
                        }
                    }
                    else if (charColour.Equals(Color.Yellow))
                    {
                        int index = diceResult == 1 ? YELLOW_HOME : (YELLOW_HOME + 6);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = Square.SquareBorder.BORDER;
                            return true;
                        }
                    }
                    else if (charColour.Equals(Color.Red))
                    {
                        int index = diceResult == 1 ? RED_HOME : (RED_HOME + 6);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = Square.SquareBorder.BORDER;
                            return true;
                        }
                    }
                    else if (charColour.Equals(Color.Blue))
                    {
                        int index = diceResult == 1 ? BLUE_HOME : (BLUE_HOME + 6);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = Square.SquareBorder.BORDER;
                            return true;
                        }
                            
                    }
                }
                else return false;
            }
            else if(charColour.Equals(Color.Green) && charPos == 39) // The green character is just outside the final stretch
            {
                if(diceResult == 6) // The green character may enter the goal
                {
                    Game.Squares[GOAL].Border = Square.SquareBorder.BORDER;
                    return true;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[(FIRST_GREEN_FINAL - 1) + diceResult], character))
                    {
                        Game.Squares[(FIRST_GREEN_FINAL - 1) + diceResult].Border = Square.SquareBorder.BORDER;
                        return true;
                    }
                }
            }
            else if (charColour.Equals(Color.Yellow) && charPos == 9) // The yellow character is just outside the final stretch
            {
                if (diceResult == 6) // The yellow character may enter the goal
                {
                    Game.Squares[GOAL].Border = Square.SquareBorder.BORDER;
                    return true;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[(FIRST_YELLOW_FINAL - 1) + diceResult], character))
                    {
                        Game.Squares[(FIRST_YELLOW_FINAL - 1) + diceResult].Border = Square.SquareBorder.BORDER;
                        return true;
                    }
                }
            }
            else if (charColour.Equals(Color.Red) && charPos == 19) // The red character is just outside the final stretch
            {
                if (diceResult == 6) // The red character may enter the goal
                {
                    Game.Squares[GOAL].Border = Square.SquareBorder.BORDER;
                    return true;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[(FIRST_RED_FINAL - 1) + diceResult], character))
                    {
                        Game.Squares[(FIRST_RED_FINAL - 1) + diceResult].Border = Square.SquareBorder.BORDER;
                        return true;
                    }
                }
            }
            else if (charColour.Equals(Color.Blue) && charPos == 29) // The blue character is just outside the final stretch
            {
                if (diceResult == 6) // The blue character may enter the goal
                {
                    Game.Squares[GOAL].Border = Square.SquareBorder.BORDER;
                    return true;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[(FIRST_BLUE_FINAL - 1) + diceResult], character))
                    {
                        Game.Squares[(FIRST_BLUE_FINAL - 1) + diceResult].Border = Square.SquareBorder.BORDER;
                        return true;
                    }
                }
            }
            else // The character is not home nor on the final stetch, see where he may move to
            {
                if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                {
                    Game.Squares[charPos + diceResult].Border = Square.SquareBorder.BORDER;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if it is possible for a specified character to move to a specified square
        /// </summary>
        /// <param name="square">The square to move to</param>
        /// <param name="character">The character to move</param>
        /// <returns>True if the character may move to the square</returns>
        private static bool CheckIfPossible(Square square, Character character)
        {
            // Check if there is a character on the square already
            if(square.State == Square.SquareState.OCCUPIED)
            {
                Color occupantColour = square.Character.Colour;

                // See if the character to be moved is of the same colour as the occupant
                if (occupantColour.Equals(character.Colour))
                {
                    return false;
                }
                else
                {
                    // See if the occupant is on a safe square
                    if(occupantColour.Equals(square.Colour))
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
        public static void MoveCharacter(Square square, Character character)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Pushes an opponent
        /// </summary>
        /// <param name="opponent">The opponent to push</param>
        private static void PushOpponent(Character opponent)
        {
            throw new NotImplementedException();
        }
    }
}
