using System;
using System.Runtime.Serialization;

namespace FiaMedKnuff
{
    internal class InvalidValueOfMaximumPlayersException : Exception
    {
        public InvalidValueOfMaximumPlayersException(string message = "Invalid value of maximum players. The value may be between 2 and 4.") : base(message)
        {
        }
    }
}