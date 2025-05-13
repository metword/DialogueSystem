using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Goto Lines when executed change control of the dialogue sequence to the
    /// specified node
    /// </summary>
    public class GotoLine : FunctionalLine
    {
        private string nodeDestination;
        private DialogueSequence sequence;

        public GotoLine(string nodeDestination, DialogueSequence sequence)
        {
            this.nodeDestination = nodeDestination;
            this.sequence = sequence;
        }

        public override void Execute()
        {
            sequence.SetCurrentNode(nodeDestination, 0);
        }
    }
}
