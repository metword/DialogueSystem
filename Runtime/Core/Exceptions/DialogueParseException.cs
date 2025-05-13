using System;

namespace DialogueSystem
{
    public class DialogueParseException : Exception
    {
        public DialogueParseException(string message) : base(message) {
           
        }
        public DialogueParseException(string message, int lineNumber) : base($"Malformed dialogue at line {lineNumber + 1}. {message}")
        {

        }
    }
}
