using System;

namespace FiaMedKnuff
{
    internal class InvalidColourException : Exception
    {
        public InvalidColourException(string message = "Invalid colour, it must be green, red, yellow or blue") : base(message)
        {
        }
    }
}