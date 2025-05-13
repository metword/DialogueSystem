using System.Collections.Generic;

namespace DialogueSystem
{
    /// <summary>
    /// A node in a dialogue tree is a collection of dialogue lines in order.
    /// </summary>
    public class Node
    {
        private List<Line> lines;

        /// <summary>
        /// Creates a node with no lines.
        /// </summary>
        public Node()
        {
            this.lines = new();
        }
        public void AddLine(Line line)
        {
            this.lines.Add(line);
        }

        /// <summary>
        /// Inserts a dialogue line into the node at the given index
        /// </summary>
        /// <param name="index">Index to insert at</param>
        /// <param name="line">Line to be inserted</param>
        public void InsertLine(int index, Line line)
        {
            this.lines.Insert(index, line);
        }

        /// <summary>
        /// Gets the line at the given index or returns null if the index is
        /// out of bounds. No array index out of bounds errors are thrown here
        /// </summary>
        /// <param name="index">Index of line to get in this node</param>
        /// <returns>The line at the given index or null</returns>
        public Line GetLine(int index) {
            if (index < 0  || index >= this.lines.Count)
            {
                return null;
            }

            return this.lines[index];
        }
    }
}
