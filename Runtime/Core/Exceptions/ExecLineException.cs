using System;

namespace DialogueSystem
{
    public class ExecLineException : Exception {
        public ExecLineException(string message, string look) : base($"Unable to parse function {look}. {message}")
        {

        } 
    }
}