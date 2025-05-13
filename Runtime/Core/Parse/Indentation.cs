namespace DialogueSystem
{
    /// <summary>
    /// Indentations used in the dialogue script
    /// </summary>
    public struct Indentation
    {
        public int IndentationChar { get; private set; }
        public int IndentationCount { get; private set; }
        public Indentation(int indentationChar, int indentationCount)
        {
            this.IndentationChar = indentationChar;
            this.IndentationCount = indentationCount;
        }
    }
}
