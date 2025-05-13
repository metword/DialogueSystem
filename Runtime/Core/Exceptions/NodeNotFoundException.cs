using System;

namespace DialogueSystem
{
    public class NodeNotFoundException : Exception
    {
        public NodeNotFoundException(string message) : base(message)
        {

        }
    }
}
