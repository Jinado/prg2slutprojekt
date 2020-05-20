using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FiaMedKnuff
{
    public static class Movement
    {
        // Constant values to help orient the map
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
        /// <param name="silent">If true, no borders will be added to squares that you may move to</param>
        /// <returns>True if there was a possible move</returns>
        public static bool DrawMovementLine(int diceResult, Character character, bool silent = false)
        {
            int charPos = character.Position;
            Color charColour = character.Colour;
            Character.CharacterState charState = character.State;

            // The character cannot move if he has already entered the goal
            if (charState == Character.CharacterState.WON) return false;

            // Check if the character is home and if he may be moved outside
            if (charState == Character.CharacterState.HOME)
            {
                if (diceResult == 1 || diceResult == 6)
                {
                    if (charColour.Equals(Color.Green))
                    {
                        int index = diceResult == 1 ? GREEN_HOME : (GREEN_HOME + 5);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                    }
                    else if (charColour.Equals(Color.Yellow))
                    {
                        int index = diceResult == 1 ? YELLOW_HOME : (YELLOW_HOME + 5);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                    }
                    else if (charColour.Equals(Color.Red))
                    {
                        int index = diceResult == 1 ? RED_HOME : (RED_HOME + 5);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                    }
                    else if (charColour.Equals(Color.Blue))
                    {
                        int index = diceResult == 1 ? BLUE_HOME : (BLUE_HOME + 5);
                        if (CheckIfPossible(Game.Squares[index], character))
                        {
                            Game.Squares[index].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                            
                    }
                }
                else return false;
            }
            else if(charColour.Equals(Color.Green) && (charPos == 39 || charPos == 38)) // The green character is just outside the final stretch
            {
                if (diceResult == 6)
                {
                    if (charPos == 38)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else if (diceResult == 5)
                {
                    if (charPos == 39)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[FIRST_GREEN_FINAL + (diceResult - 1)], character))
                    {
                        Game.Squares[FIRST_GREEN_FINAL + (diceResult - 1)].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
            }
            else if (charColour.Equals(Color.Yellow) && (charPos == 9 || charPos == 8)) // The yellow character is just outside the final stretch
            {
                if (diceResult == 6)
                {
                    if (charPos == 8)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else if (diceResult == 5)
                {
                    if (charPos == 9)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[FIRST_YELLOW_FINAL + (diceResult - 1)], character))
                    {
                        Game.Squares[FIRST_YELLOW_FINAL + (diceResult - 1)].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
            }
            else if (charColour.Equals(Color.Red) && (charPos == 19 || charPos == 18)) // The red character is just outside the final stretch
            {
                if (diceResult == 6)
                {
                    if(charPos == 18)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else if (diceResult == 5) 
                {
                    if (charPos == 19)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[FIRST_RED_FINAL + (diceResult - 1)], character))
                    {
                        Game.Squares[FIRST_RED_FINAL + (diceResult - 1)].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
            }
            else if (charColour.Equals(Color.Blue) && (charPos == 29 || charPos == 28)) // The blue character is just outside the final stretch
            {
                if (diceResult == 6)
                {
                    if (charPos == 28)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else if (diceResult == 5)
                {
                    if (charPos == 29)
                    {
                        Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                    else return false;
                }
                else
                {
                    if (CheckIfPossible(Game.Squares[FIRST_BLUE_FINAL + (diceResult - 1)], character))
                    {
                        Game.Squares[FIRST_BLUE_FINAL + (diceResult - 1)].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
            }
            else if(charColour.Equals(Color.Green) && Game.Squares[charPos].Path == Square.PathType.FINAL) // The green character is somewhere in his final stretch
            {
                // The indexes for the final stretch for green are 40, 41, 42, 43
                switch (charPos)
                {
                    case 40:
                        if (diceResult > 4) return false;
                        else if (diceResult == 4)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 41:
                        if (diceResult > 3) return false;
                        else if (diceResult == 3)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 42:
                        if (diceResult > 2) return false;
                        else if (diceResult == 2)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 43:
                        if (diceResult > 1) return false;
                        else
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                }

                return false;
            }
            else if (charColour.Equals(Color.Yellow) && Game.Squares[charPos].Path == Square.PathType.FINAL) // The yellow character is somewhere in his final stretch
            {
                // The indexes for the final stretch for yellow are 44, 45, 46, 47
                switch (charPos)
                {
                    case 44:
                        if (diceResult > 4) return false;
                        else if (diceResult == 4)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 45:
                        if (diceResult > 3) return false;
                        else if (diceResult == 3)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 46:
                        if (diceResult > 2) return false;
                        else if (diceResult == 2)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 47:
                        if (diceResult > 1) return false;
                        else
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                }

                return false;
            }
            else if (charColour.Equals(Color.Red) && Game.Squares[charPos].Path == Square.PathType.FINAL) // The red character is somewhere in his final stretch
            {
                // The indexes for the final stretch for red are 48, 49, 50, 51
                switch (charPos)
                {
                    case 48:
                        if (diceResult > 4) return false;
                        else if (diceResult == 4)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 49:
                        if (diceResult > 3) return false;
                        else if (diceResult == 3)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 50:
                        if (diceResult > 2) return false;
                        else if (diceResult == 2)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 51:
                        if (diceResult > 1) return false;
                        else
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                }

                return false;
            }
            else if (charColour.Equals(Color.Blue) && Game.Squares[charPos].Path == Square.PathType.FINAL) // The blue character is somewhere in his final stretch
            {
                // The indexes for the final stretch for blue are 52, 53, 54, 55
                switch (charPos)
                {
                    case 52:
                        if (diceResult > 4) return false;
                        else if (diceResult == 4)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 53:
                        if (diceResult > 3) return false;
                        else if (diceResult == 3)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 54:
                        if (diceResult > 2) return false;
                        else if (diceResult == 2)
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                        else
                        {
                            if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                            {
                                Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                                return true;
                            }
                        }
                        break;
                    case 55:
                        if (diceResult > 1) return false;
                        else
                        {
                            Game.Squares[GOAL].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                            return true;
                        }
                }

                return false;
            }
            else // The character is not home nor on the final stretch, see where he may move to
            {
                // Check if the current character IS NOT green and if this character is able to move past square #39
                // If he can, make sure the counting gets reset and begins from 0 as not to enter green's final stretch
                if(!charColour.Equals(Color.Green) && (charPos + diceResult) > 39)
                {
                    int stepsTo39 = 39 - charPos;
                    int stepsLeftOnDice = diceResult - stepsTo39;
                    if(CheckIfPossible(Game.Squares[-1 + stepsLeftOnDice], character))
                    {
                        Game.Squares[-1 + stepsLeftOnDice].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
                // Look if the characters are able to enter their final stretch
                else if (charColour.Equals(Color.Yellow) && (charPos + diceResult) > 9 && charPos < 9)
                {
                    int stepsTo9 = 9 - charPos;
                    int stepsLeftOnDice = diceResult - stepsTo9;

                    if (CheckIfPossible(Game.Squares[FIRST_YELLOW_FINAL + (stepsLeftOnDice - 1)], character))
                    {
                        Game.Squares[FIRST_YELLOW_FINAL + (stepsLeftOnDice - 1)].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
                else if (charColour.Equals(Color.Red) && (charPos + diceResult) > 19 && charPos < 19)
                {
                    int stepsTo19 = 19 - charPos;
                    int stepsLeftOnDice = diceResult - stepsTo19;
                    if (CheckIfPossible(Game.Squares[FIRST_RED_FINAL + (stepsLeftOnDice - 1)], character))
                    {
                        Game.Squares[FIRST_RED_FINAL + (stepsLeftOnDice - 1)].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
                else if (charColour.Equals(Color.Blue) && (charPos + diceResult) > 29 && charPos < 29)
                {
                    int stepsTo29 = 29 - charPos;
                    int stepsLeftOnDice = diceResult - stepsTo29;
                    if (CheckIfPossible(Game.Squares[FIRST_BLUE_FINAL + (stepsLeftOnDice - 1)], character))
                    {
                        Game.Squares[FIRST_BLUE_FINAL + (stepsLeftOnDice - 1)].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
                else if ((charPos + diceResult) <= 56) // Make sure they cannot move past the goal
                {
                    if (CheckIfPossible(Game.Squares[charPos + diceResult], character))
                    {
                        Game.Squares[charPos + diceResult].Border = !silent ? Square.SquareBorder.BORDER : Square.SquareBorder.BORDERLESS;
                        return true;
                    }
                }
                else return false;
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
        /// <param name="pathLocation">The location of the squares's picturebox</param>
        /// <param name="pbxCharacter">The character's picturebox</param>
        public static void MoveCharacter(Square square, Character character, Point pathLocation, PictureBox pbxCharacter)
        {
            // Change the state of the square the character to be moved is standning on to free
            // as he is now moving away from it
            if(character.Position != -1)
                Game.Squares[character.Position].State = Square.SquareState.FREE;

            // Remove the border from the square
            square.Border = Square.SquareBorder.BORDERLESS;

            // Position the character in the middle of the path he went to (The character is shrunk down to half its size when it leaves its home)
            pbxCharacter.Size = new Size(32, 30);
            pbxCharacter.Location = new Point(pathLocation.X + 16, pathLocation.Y + 15);

            // Check the colour of the new square the character has moved to and make sure his picturebox's BackColor matches with the square's
            switch (Character.ColourToString(square.Colour))
            {
                case "Green":
                    pbxCharacter.BackColor = Character.GREEN;
                    break;
                case "Yellow":
                    pbxCharacter.BackColor = Character.YELLOW;
                    break;
                case "Red":
                    pbxCharacter.BackColor = Character.RED;
                    break;
                case "Blue":
                    pbxCharacter.BackColor = Character.BLUE;
                    break;
                case "Black":
                    pbxCharacter.BackColor = Character.BLACK;
                    break;
            }

            // Make sure the characters state is marked as "outside"
            character.State = Character.CharacterState.OUTSIDE;

            if(square.State == Square.SquareState.FREE)
                square.State = Square.SquareState.OCCUPIED;
            else
                PushOpponent(square.Character);

            // Only change the character's position to the new position AFTER
            // the character has potentially pushed someone else. Otherwise both
            // the character and the opponent would have the same position at the
            // same time, which leads to both characters being sent back to spawn
            character.Position = square.Position;
            pbxCharacter.Tag = character.Position;
            square.Character = character;
        }

        /// <summary>
        /// Pushes an opponent
        /// </summary>
        /// <param name="opponent">The opponent to push</param>
        private static void PushOpponent(Character opponent)
        {
            PictureBox pbxOpponent = null;
            foreach(PictureBox pbx in Characters)
            {
                if (pbx.Tag.Equals(opponent.Position))
                {
                    pbxOpponent = pbx;
                    break;
                }
            }

            pbxOpponent.Tag = -1;
            pbxOpponent.Size = new Size(64, 60);
            // Place the picturebox at an empty location in its spawn
            switch (Character.ColourToString(opponent.Colour))
            {
                case "Green":
                    if (Form.GetChildAtPoint(Character.GREEN_POS1).Name == "pbxSpawnPointGreen")
                        pbxOpponent.Location = Character.GREEN_POS1;
                    else if (Form.GetChildAtPoint(Character.GREEN_POS2).Name == "pbxSpawnPointGreen")
                        pbxOpponent.Location = Character.GREEN_POS2;
                    else if (Form.GetChildAtPoint(Character.GREEN_POS3).Name == "pbxSpawnPointGreen")
                        pbxOpponent.Location = Character.GREEN_POS3;
                    else if (Form.GetChildAtPoint(Character.GREEN_POS4).Name == "pbxSpawnPointGreen")
                        pbxOpponent.Location = Character.GREEN_POS4;

                    pbxOpponent.BackColor = Character.GREEN;
                    break;
                case "Yellow":
                    if (Form.GetChildAtPoint(Character.YELLOW_POS1).Name == "pbxSpawnPointYellow")
                        pbxOpponent.Location = Character.YELLOW_POS1;
                    else if (Form.GetChildAtPoint(Character.YELLOW_POS2).Name == "pbxSpawnPointYellow")
                        pbxOpponent.Location = Character.YELLOW_POS2;
                    else if (Form.GetChildAtPoint(Character.YELLOW_POS3).Name == "pbxSpawnPointYellow")
                        pbxOpponent.Location = Character.YELLOW_POS3;
                    else if (Form.GetChildAtPoint(Character.YELLOW_POS4).Name == "pbxSpawnPointYellow")
                        pbxOpponent.Location = Character.YELLOW_POS4;

                    pbxOpponent.BackColor = Character.YELLOW;
                    break;
                case "Red":
                    if (Form.GetChildAtPoint(Character.RED_POS1).Name == "pbxSpawnPointRed")
                        pbxOpponent.Location = Character.RED_POS1;
                    else if (Form.GetChildAtPoint(Character.RED_POS2).Name == "pbxSpawnPointRed")
                        pbxOpponent.Location = Character.RED_POS2;
                    else if (Form.GetChildAtPoint(Character.RED_POS3).Name == "pbxSpawnPointRed")
                        pbxOpponent.Location = Character.RED_POS3;
                    else if (Form.GetChildAtPoint(Character.RED_POS4).Name == "pbxSpawnPointRed")
                        pbxOpponent.Location = Character.RED_POS4;

                    pbxOpponent.BackColor = Character.RED;
                    break;
                case "Blue":
                    if (Form.GetChildAtPoint(Character.BLUE_POS1).Name == "pbxSpawnPointBlue")
                        pbxOpponent.Location = Character.BLUE_POS1;
                    else if (Form.GetChildAtPoint(Character.BLUE_POS2).Name == "pbxSpawnPointBlue")
                        pbxOpponent.Location = Character.BLUE_POS2;
                    else if (Form.GetChildAtPoint(Character.BLUE_POS3).Name == "pbxSpawnPointBlue")
                        pbxOpponent.Location = Character.BLUE_POS3;
                    else if (Form.GetChildAtPoint(Character.BLUE_POS4).Name == "pbxSpawnPointBlue")
                        pbxOpponent.Location = Character.BLUE_POS4;

                    pbxOpponent.BackColor = Character.BLUE;
                    break;
            }

            // The opponent is pushed back to his home, so his state must change
            opponent.State = Character.CharacterState.HOME;
            opponent.Position = -1;
        }


        // The FrmGame form to be able to access its list of Controls
        public static FrmGame Form { get; set; }
        public static List<PictureBox> Characters { get; set; }
    }
}
