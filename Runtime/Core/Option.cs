
using System;

namespace DialogueSystem
{
    /// <summary>
    /// An option in the optional line contains the text contents and the
    /// node that the option would change execution towards.
    /// </summary>
    public class Option
    {
        public string Node { get; private set; }
        public string Text { get; private set; }
        public string Id { get; set; }
        public bool Enabled { get; set; }

        /// <summary>
        /// Creates a new Option
        /// </summary>
        /// <param name="node">Target node of the option choice</param>
        /// <param name="text">text to be displayed on the option</param>
        public Option(string node, string text)
        {
            this.Node = node;
            this.Text = text;
            this.Enabled = true;
        }

        public override string ToString()
        {
            return $"-> {Text} | {Node}";
        }
    }
}