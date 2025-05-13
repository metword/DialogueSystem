using DialogueSystem;
using System;

namespace DialogueSystem {
    /// <summary>
    /// A dialogue reader reads input from a dialogue sequence
    /// </summary>
    public interface IDialogueReader
    {
        /// <summary>
        /// Reads a single line from the dialogue sequence. Invokes the callback
        /// when the line is ready to be dismissed.
        /// <br></br>
        /// If working with a dialogue system that needed a click for example to
        /// move onto the next line, the callback should be invoked onclick. 
        /// </summary>
        /// <param name="line">Line to be displayed</param>
        /// <param name="callback">Callback to invoke on line display</param>
        public void ReadLine(DialogueLine line, Action callback);

        /// <summary>
        /// Reads an optional line from the dialogue sequence. Invokes the
        /// callback when then options are ready to be dismissed.
        /// <br></br>
        /// The callback contains a string parameter. This parameter is
        /// </summary>
        /// <param name="option">Optional line being selected</param>
        /// <param name="callback">Callback invoked when the option is selected
        /// the string parameter is the selected node</param>
        public void ReadOption(OptionalLine option, Action<string> callback);

        /// <summary>
        /// Sent by the dialogue sequence when dialogue has ended.
        /// </summary>
        public void ReadEnd();
    }
}
