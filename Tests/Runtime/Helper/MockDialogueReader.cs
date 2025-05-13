using DialogueSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogueSystemRuntimeTests
{
    /// <summary>
    /// Simple class used for reading dialogue in a testing environment
    /// </summary>
    public class MockDialogueReader : IDialogueReader
    {
        private DialogueLine line;
        private OptionalLine option;

        private Action lineCallback;
        private Action<string> optionCallback;

        public MockDialogueReader()
        {
            this.line = null;
            this.option = null;
        }
        public void ReadLine(DialogueLine line, Action callback)
        {
            this.line = line;
            this.lineCallback = callback;
        }

        public void ReadOption(OptionalLine option, Action<string> callback)
        {
            this.option = option;
            this.optionCallback = callback;
        }

        public void NextLine()
        {
            lineCallback.Invoke();
        }

        public void NextRandomOption()
        {
            int index = UnityEngine.Random.Range(0, option.GetOptions().Keys.Count);
            Option opt = option.GetOptions().Values.ToList()[index];
            optionCallback.Invoke(opt.Node);
        }

        public void NextOption(string optionName)
        {
            optionCallback.Invoke(optionName);
        }

        public void NextOption(int optionId)
        {
            optionCallback.Invoke(option.GetOption(optionId).Node);
        }

        /// <summary>
        /// Get the line currenly being displayed 
        /// </summary>
        /// <returns>The line</returns>
        public DialogueLine GetLine()
        {
            return line;
        }

        /// <summary>
        /// Get the option currenly being displayed 
        /// </summary>
        /// <returns>The line</returns>
        public OptionalLine GetOption()
        {
            return option;
        }

        public void ReadEnd()
        {
            this.line = null;
            this.option = null;
            this.lineCallback = null;
            this.optionCallback = null;
        }

    }
}
