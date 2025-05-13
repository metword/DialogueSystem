using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Exec lines execute a user defined function when read in the dialogue
    /// system
    /// </summary>
    public class ExecLine : FunctionalLine
    {
        private string function;
        private string[] parameters;
        private DialogueSequence sequence;
        public ExecLine(string function, string[] parameters, DialogueSequence sequence)
        {
            this.function = function;
            this.parameters = parameters;
            this.sequence = sequence;
        }
        public override void Execute()
        {
            sequence.InvokeFunction(function, parameters);
        }
        public string[] GetParameters()
        {
            return this.parameters;
        }

    }
}