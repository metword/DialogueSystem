using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// A functional line is not displayed in the dialouge system but rather
    /// executes user specified functions on the dialogue sequence.
    /// </summary>
    public abstract class FunctionalLine : Line
    {
        /// <summary>
        /// Executes the given function specified on this Functional line
        /// </summary>
        public abstract void Execute();
    }
}