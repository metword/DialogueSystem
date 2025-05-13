namespace DialogueSystem
{
    /// <summary>
    /// A regular line in the dialogue tree that displays a single line
    /// </summary>
    public class DialogueLine : Line
    {
        private string speaker;
        private string text;

        /// <summary>
        /// Creates a new dialogue line with given text and speaker.
        /// </summary>
        /// <param name="text">Text being said</param>
        /// <param name="speaker">Who is saying this dialogue line</param>
        public DialogueLine(string speaker, string text)
        {
            this.speaker = speaker;
            this.text = text;
        }

        /// <summary>
        /// Text contents of this Dialogue line
        /// </summary>
        /// <returns></returns>
        public string GetText()
        {
            return text;
        }

        /// <summary>
        /// Speaker of this line
        /// </summary>
        /// <returns></returns>
        public string GetSpeaker()
        {
            return speaker;
        }

        public override string ToString()
        {
            return $"DialogueLine\n{speaker}: {text}";
        }
    }
}
