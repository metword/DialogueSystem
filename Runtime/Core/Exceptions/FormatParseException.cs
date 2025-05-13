using System;


namespace DialogueSystem
{
    /// <summary>
    /// Thrown if a string is not formatted properly to be parsed using the
    /// format parser.
    /// </summary>
    public class FormatParseException : Exception
    {
        public FormatParseException(string message) : base(message) { 

        }
    }

}
