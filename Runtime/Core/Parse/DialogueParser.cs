using DialogueSystem.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// This script handles the parsing of strings into a node based dialogue 
    /// tree that can be used by the main dialogue system
    /// </summary>
    public class DialogueParser
    {
        private List<Command> commands;
        private int anonId;
        private List<List<ParsedNode>> layers;
        private Dictionary<string, ParsedNode> nodes;
        private HashSet<int> breaks;
        public DialogueParser()
        {
            commands = CommandSettings.GetCommands();
            anonId = 0;
            layers = new();
            nodes = new();
            breaks = new();
        }

        /// <summary>
        /// Parsed the given string formatted dialogue into a dialogue sequence
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public DialogueSequence Parse(string input)
        {
            if (input == null)
            {
                throw new DialogueParseException("Input cannot be null");
            }

            // go through line by line and build up the sequence
            DialogueSequence sequence = new();

            string[] lines = input.Split('\n');

            // 2 step parse -->
            //      1. Build branches
            //      2. Build the sequence with the branches

            // branches start with options. If non-options are added, we will
            // create a new branch when options are added again.

            // first pass to create the branches
            for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
            {
                string line = lines[lineNumber];
                // skip blank lines
                if (string.IsNullOrEmpty(line.Trim())) continue;

                // create all our parsed Nodes
                ParseNodes(line, lineNumber, sequence);
            }

            // second pass throught the parsed nodes to create the sequence
            foreach (List<ParsedNode> list in layers)
            {
                foreach (ParsedNode node in list)
                {
                    BuildSequence(node, sequence);
                }
            }

            // set the start of sequence
            string startNode = layers[0].First().GetNodeKey();
            sequence.SetCurrentNode(startNode);
            return sequence;
        }

        private void BuildSequence(ParsedNode node, DialogueSequence sequence)
        {
            string nodeName = node.GetNodeKey();

            sequence.AddNode(nodeName);
            foreach (Line line in node.GetLines())
            {
                sequence.AddLine(nodeName, line);
            }

            // if it has an option, we don't add any gotos or anything
            if (node.HasOption())
            {
                sequence.AddLine(nodeName, node.GetOption());
            }
            // if not, we need to create a goto node to the node AFTER parent (if there is a parent)
            else
            {
                ParsedNode parent = node.GetParentNode();
                if (parent != null)
                {
                    int parentLayer = parent.GetLayer();
                    int afterParentIndex = parent.GetIndex() + 1;
                    if (afterParentIndex < layers[parentLayer].Count)
                    {
                        string destinationNode = layers[parentLayer][afterParentIndex].GetNodeKey();
                        sequence.AddLine(nodeName, new GotoLine(destinationNode, sequence));
                    }
                }
            }

        }

        /// <summary>
        /// Gets the indentation level of the line, where 0 is no indentation
        /// and 1 is # spaces or 1 tab.
        /// </summary>
        /// <param name="line">Line to get indentation level from</param>
        /// <param name="lineAfterIndent">The line text after the indentation
        /// </param>
        /// <returns>The indentation level integer or -1 if malformed</returns>
        private int GetIndentationLevel(string line, out string lineAfterIndent)
        {
            if (line.Length == 0)
            {
                lineAfterIndent = line;
                return 0;
            }

            int indentationCount = 0;
            bool indented = false;
            Indentation targetIndentation = new();
            foreach (Indentation indent in CommandSettings.GetIndentations())
            {
                if (line[0] == indent.IndentationChar)
                {
                    targetIndentation = indent;
                    indented = true;
                    break;
                }
            }

            if (!indented)
            {
                lineAfterIndent = line;
                return 0;
            }


            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == targetIndentation.IndentationChar)
                {
                    indentationCount++;
                }
                else
                {
                    // check that mix of indents not present
                    if (CommandSettings.GetIndentations().Any(ind =>
                    {
                        // if it matches another indentation char, there was
                        // a fail mix of tabs and spaces
                        return line[i] == ind.IndentationChar;
                    }))
                    {
                        throw new DialogueParseException($"Mix of tabs and spaces: {line}");
                    }
                    break;
                }
            }

            // indentation level is based on how many chars per indent
            int level = indentationCount / targetIndentation.IndentationCount;
            int remainder = indentationCount % targetIndentation.IndentationCount;

            if (remainder == 0)
            {
                lineAfterIndent = line[indentationCount..];
                return level;
            }

            throw new DialogueParseException($"Uneven number of indentations {line}");
        }

        /// <summary>
        /// Parse a given string into a node with dialogue lines present on it
        /// </summary>
        /// <param name="line"></param>
        private void ParseNodes(string line, int lineNumber, DialogueSequence sequence)
        {
            int lineIndentation = GetIndentationLevel(line, out string afterIndent);
            string toParse = afterIndent.Trim();
            Command escape = CommandSettings.GetCommand(CommandType.Escape);

            (string before, string after, string next, Command command) = toParse.SplitCommand(escape, commands);
            before = before.Trim();
            after = after.Trim();
            next = next.Trim();
            Line createdLine = null;
            int createdOptionId = -1;

            int iterations = 0;
            while (command != null && iterations < 1000)
            {
                iterations++;
                if (command.Type == CommandType.NodeStart)
                {
                    // reset the map and hashset so we can add the new parsed node at the indentation level 
                    // nodes always exist without a parent
                    breaks.Remove(lineIndentation);
                    if (commands.Any(c => after.Contains(c.CommandString)))
                        throw new DialogueParseException("Node id cannot contain any other commands", lineNumber);
                    ParsedNode created = CreateParsedNode(after, null, lineIndentation, lineNumber);
                    toParse = "";
                }
                else if (command.Type == CommandType.Option)
                {
                    // we know to break out of this node whenever going to a new non-option line
                    breaks.Add(lineIndentation);

                    ParsedNode topParsedNode = GetTopLayer(lineIndentation, lineNumber);

                    // create the suboption
                    string anonNode = GetAnonNode();
                    ParsedNode subNode = CreateParsedNode(anonNode, topParsedNode, lineIndentation + 1, lineNumber);

                    createdOptionId = topParsedNode.AddOption(anonNode, after);

                    createdLine = topParsedNode.GetOption();

                    // when hitting a option, subsequent commands have
                    // an indentation level up 1
                    lineIndentation++;
                    toParse = next;
                }
                else if (command.Type == CommandType.IdStart)
                {
                    // next must be an id end or else it is unclosed
                    if (createdLine == null)
                    {
                        throw new DialogueParseException("Ids must come after command elements", lineNumber);
                    }
                    (_, _, _, Command nextCommand) = next.SplitCommand(escape, commands);
                    if (nextCommand == null || nextCommand.Type != CommandType.IdEnd)
                    {
                        throw new DialogueParseException("Unclosed id", lineNumber);
                    }
                    toParse = next[nextCommand.CommandString.Length..];
                    createdLine.AddId(after);
                    if (createdLine is OptionalLine oLine)
                    {
                        oLine.GetOption(createdOptionId).Id = after;
                    }
                }
                else if (command.Type == CommandType.IdEnd)
                {
                    throw new DialogueParseException("Unopened id", lineNumber);
                }
                else if (command.Type == CommandType.Comment)
                {
                    toParse = "";
                }
                else if (command.Type == CommandType.Dialogue)
                {
                    Line newLine = new DialogueLine(before, after);
                    createdLine = newLine;

                    AddToNode(newLine, lineIndentation, lineNumber);
                    toParse = next;
                }
                else if (command.Type == CommandType.Goto)
                {
                    Line newLine = new GotoLine(after, sequence);
                    createdLine = newLine;

                    AddToNode(newLine, lineIndentation, lineNumber);
                    toParse = next;
                }
                else if (command.Type == CommandType.Exec)
                {
                    (string, string[]) func = GetFunctionAndParameters(after);
                    Line newLine = new ExecLine(func.Item1, func.Item2, sequence);
                    createdLine = newLine;

                    AddToNode(newLine, lineIndentation, lineNumber);
                    toParse = next;
                } else
                {
                    // unknown command
                    Debug.LogWarning($"Unknown command {command}");
                    toParse = "";
                }

                // resplit
                (before, after, next, command) = toParse.SplitCommand(escape, commands);
                before = before.Trim();
                after = after.Trim();
                next = next.Trim();
            }
        }

        /// <summary>
        /// Tries to find command within the string at index provided
        /// </summary>
        /// <param name="searchString">String where search within</param>
        /// <returns>The command if found, else returns null</returns>

        private Command GetCommand(string searchString)
        {
            foreach (Command searchingCommands in commands)
            {
                // UnityEngine.Debug.Log($"Searching {searchingCommands.Type} {searchingCommands.CommandString}");
                if (searchString.StartsWith(searchingCommands.CommandString))
                    return searchingCommands;
            }

            return null;
        }

        /// <summary>
        /// Gets the next available anonymous node name
        /// </summary>
        /// <returns>The node name</returns>
        private string GetAnonNode()
        {
            return $"<{anonId++}>";
        }

        /// <summary>
        /// Determines if the given string starts with any of the commands
        /// </summary>
        /// <param name="str">String to check</param>
        /// <param name="foundCommand">The command that was found</param>
        /// <returns></returns>
        private bool StartsWithCommand(string str, List<Command> searchCommands, out string foundCommand)
        {
            foreach (Command command in searchCommands)
            {
                if (str.StartsWith(command.CommandString))
                {
                    foundCommand = command.CommandString;
                    return true;
                }
            }
            foundCommand = null;
            return false;
        }

        /// <summary>
        /// Creates a parsed node added to the DialogueParser with the given key and indentation
        /// </summary>
        /// <param name="nodeKey">Key of parsed node being created</param>
        /// <param name="lineIndentation">indentation level of the node</param>
        /// <returns>Returns the created node</returns>
        private ParsedNode CreateParsedNode(string nodeKey, ParsedNode parentNode, int lineIndentation, int lineNumber)
        {
            if (nodeKey == null)
                throw new ArgumentNullException("Node key cannot be null when creating parsed nodes");

            TryCreateLayer(lineIndentation, lineNumber);
            int index = layers[lineIndentation].Count;
            ParsedNode newParsedNode = new(nodeKey, parentNode, lineIndentation, index);

            layers[lineIndentation].Add(newParsedNode);
            // Debug.Log("Creating parsed node " + nodeKey);
            nodes.Add(nodeKey, newParsedNode);
            return newParsedNode;
        }

        private void TryCreateLayer(int lineIndentation, int lineNumber)
        {
            if (lineIndentation > layers.Count)
                throw new DialogueParseException("Gap in layers", lineNumber);

            if (lineIndentation == layers.Count)
                layers.Add(new());
        }

        private ParsedNode GetTopLayer(int lineIndentation, int lineNumber)
        {
            if (layers[lineIndentation].Count == 0)
                throw new DialogueParseException($"Layer not present at indent level {lineIndentation}", lineNumber);

            ParsedNode topParsedNode = layers[lineIndentation].Last();

            return topParsedNode;
        }

        private void AddToNode(Line line, int lineIndentation, int lineNumber)
        {
            // create a new node at layer
            if (breaks.Contains(lineIndentation))
            {
                ParsedNode previousNode = GetTopLayer(lineIndentation, lineNumber);
                ParsedNode newNode = CreateParsedNode(GetAnonNode(), previousNode.GetParentNode(), lineIndentation, lineNumber);
                newNode.AddLine(line);
                
                breaks.Remove(lineIndentation);

            }
            else
            {
                if (lineIndentation >= layers.Count)
                    throw new DialogueParseException("Line must be connected to a node", lineNumber);

                List<ParsedNode> nodes = layers[lineIndentation];

                if (nodes.Count == 0)
                    throw new DialogueParseException("Line must be connected to a node", lineNumber);

                nodes.Last().AddLine(line);
            }
        }
        private (string, string[]) GetFunctionAndParameters(string functionText)
        {
            StringBuilder buffer = new();
            List<Command> parts = CommandSettings.GetFunctionParts();
            bool readingParams = false;
            List<string> createdParams = new();
            bool parsing = true;
            int index = 0;
            string outputFunction = null;
            while (parsing)
            {
                Command foundCommand = null;
                if (index >= functionText.Length)
                {
                    parsing = false;
                }
                else
                {
                    string look = functionText[index..];

                    foreach (Command command in parts)
                    {
                        if (look.StartsWith(command.CommandString))
                        {
                            foundCommand = command;
                        }
                    }
                }

                if (foundCommand == null && parsing)
                {
                    buffer.Append(functionText[index]);
                    index++;
                }

                if (foundCommand != null)
                {
                    string bufferOutput = buffer.ToString().Trim();
                    if (foundCommand.Type == CommandType.Escape)
                    {
                        int escapeLength = foundCommand.CommandString.Length;
                        bool spaceAfter = functionText.Length > index + foundCommand.CommandString.Length;
                        if (!spaceAfter)
                        {
                            throw new ExecLineException("Unexpected escape character", functionText);
                        }

                        string after = functionText[(index + escapeLength)..];
                        if (StartsWithCommand(after, parts, out string commandString))
                        {
                            index += commandString.Length;
                        }
                        else
                        {
                            throw new ExecLineException("Unknown escape", functionText);
                        }
                    }
                    else if (foundCommand.Type == CommandType.FunctionStart)
                    {
                        if (readingParams)
                            throw new ExecLineException("Function opened twice", functionText);

                        readingParams = true;
                        outputFunction = bufferOutput;
                        buffer.Clear();
                    }
                    else if (foundCommand.Type == CommandType.FunctionEnd)
                    {
                        if (!readingParams)
                            throw new ExecLineException("Unopened function", functionText);

                        if (!string.IsNullOrEmpty(bufferOutput))
                            createdParams.Add(bufferOutput);
                        buffer.Clear();
                        parsing = false;
                    }
                    else if (foundCommand.Type == CommandType.ParamDelim)
                    {
                        if (!readingParams)
                            throw new ExecLineException("Unexpected parameter delim", functionText);

                        if (!string.IsNullOrEmpty(bufferOutput))
                            createdParams.Add(bufferOutput);
                        buffer.Clear();
                    }

                    index += foundCommand.CommandString.Length;
                }
            }

            return (outputFunction, createdParams.ToArray());
        }

        private class ParsedNode
        {
            private OptionalLine option;
            private List<Line> lines;
            private string nodeKey;
            private ParsedNode parentNode;
            private int layerIndex;
            private int layer;

            public ParsedNode(string nodeKey, ParsedNode parentNode, int layer, int layerIndex)
            {
                this.nodeKey = nodeKey;
                this.parentNode = parentNode;
                this.layer = layer;
                this.layerIndex = layerIndex;
                this.lines = new();

                // option starts out undefined
                option = null;
            }
            public string GetNodeKey()
            {
                return nodeKey;
            }
            public ParsedNode GetParentNode()
            {
                return parentNode;
            }
            public void AddLine(Line line)
            {
                this.lines.Add(line);
            }
            public List<Line> GetLines()
            {
                return lines;
            }
            public bool HasOption()
            {
                return option != null;
            }
            public int AddOption(string node, string text)
            {
                this.option ??= new();
                return this.option.AddOption(node, text);
            }
            public OptionalLine GetOption()
            {
                return option;
            }

            public int GetIndex()
            {
                return layerIndex;
            }

            public int GetLayer()
            {
                return layer;
            }
        }
    }
}
