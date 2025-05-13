using System;

namespace DialogueSystem.Utils
{
    /// <summary>
    /// Thrown if String Utilities failed
    /// </summary>
    public class StringUtilsException : Exception
    {
        public StringUtilsException(string message) : base(message)
        {

        }
    }
}
