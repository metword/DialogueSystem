using System;
using System.Collections.Generic;

namespace DialogueSystem
{
    /// <summary>
    /// Dialogue sequence handles stepping through dialogue. It is the main
    /// container outlining dialogue flow.
    /// </summary>
    public class DialogueSequence
    {
        private Dictionary<string, Delegate> functionMap = new();
        private Dictionary<string, Line> idLines;
        private List<IDialogueReader> dialogueReaders;
        private Dictionary<string, Node> nodes;
        private string currentNode;
        private int lineIndex;

        public DialogueSequence()
        {
            this.idLines = new();
            this.functionMap = new();
            this.dialogueReaders = new();
            this.nodes = new();
        }

        /// <summary>
        /// Starts the dialogue sequence. Will warn the user if any variables
        /// have not been set yet.
        /// </summary>
        public void StartSequence()
        {
            if (currentNode == null) {
                throw new DialogueSequenceException("Current node not set to start sequence");
            }

            if (!nodes.ContainsKey(currentNode))
            {
                throw new NodeNotFoundException($"Starting node not found in nodes '{currentNode}'");
            }

            // todo: warn variables not present

            UpdateSequence();
        }

        /// <summary>
        /// Updates the sequence to display the correct, currently visble line
        /// </summary>
        private void UpdateSequence()
        {
            // advance until on a non-functional line so that we can then
            // display that line
            Line firstLine = AdvanceToNextVisibleLine();

            DisplayToReaders(firstLine);
        }

        /// <summary>
        /// Advances execution to the next visible line in the dialogue 
        /// sequence. Visible lines are dialogue lines or optional lines. Lines
        /// that have text that can be displayed to the screen. Executes any
        /// functional lines along the way
        /// </summary>
        /// <returns>The found visible</returns>
        private Line AdvanceToNextVisibleLine()
        {
            Node node = nodes[currentNode];
            Line line = node.GetLine(lineIndex);

            while (line is FunctionalLine func)
            {
                lineIndex++;
                // exec lines will execute their functions and goto lines will
                // change line index and currentnode so then below we can
                // update
                func.Execute();
                node = nodes[currentNode];
                line = node.GetLine(lineIndex);
            }

            return line;
        }

        /// <summary>
        /// Displays the given line to the readers listening on this sequence
        /// </summary>
        /// <param name="line"></param>
        private void DisplayToReaders(Line line)
        {
            if (line is OptionalLine oLine)
            {
                DisplayOptionalLine(oLine);
            } 
            else if (line is DialogueLine dLine)
            {
                DisplayDialogueLine(dLine);
            } 
            else
            {
                DisplayEnd();
                // unknown display
            }
        }

        private void DisplayOptionalLine(OptionalLine oLine)
        {
            foreach (IDialogueReader reader in this.dialogueReaders)
            {
                // when the option is selected go to the options target node
                reader.ReadOption(oLine, (node) =>
                {
                    SetCurrentNode(node, 0);
                    UpdateSequence();
                });
            }
        }

        private void DisplayDialogueLine(DialogueLine dLine)
        {
            foreach (IDialogueReader reader in this.dialogueReaders)
            {
                // when the option is selected go to the options target node
                reader.ReadLine(dLine, () =>
                {
                    lineIndex++;
                    UpdateSequence();
                });
            }
        }

        private void DisplayEnd()
        {
            foreach (IDialogueReader reader in this.dialogueReaders)
            {
                reader.ReadEnd();
            }
        }

        /// <summary>
        /// Adds a node to the sequence with no lines
        /// </summary>
        /// <param name="nodeKey"></param>
        /// <exception cref="ArgumentException">Throws if key trying to be 
        /// added already exists</exception>
        /// <exception cref="ArgumentNullException">Thrown if key is null
        /// </exception>
        public void AddNode(string nodeKey)
        {
            if (nodeKey == null)
                throw new ArgumentNullException("Node cannot be null");

            if (this.nodes.ContainsKey(nodeKey))
            {
                throw new ArgumentException("Key already exists");
            }

            this.nodes.Add(nodeKey, new Node());
        }

        /// <summary>
        /// Adds a line to the dialogue sequence off of the given node. Creates
        /// a new node in the sequence if the node does not exist
        /// </summary>
        /// <param name="node"></param>
        /// <param name="line"></param>
        /// <exception cref="ArgumentException">Thrown if the node does not 
        /// exist</exception>
        public void AddLine(string nodeKey, Line line)
        {
            if (!nodes.ContainsKey(nodeKey)) {
                throw new ArgumentException("Node does not exist");
            }

            foreach (string id in line.GetIds())
            {
                idLines.Add(id, line);
            }

            Node toAdd = nodes[nodeKey];
            toAdd.AddLine(line);
        }

        /// <summary>
        /// Sets the current node for this dialogue sequence
        /// </summary>
        /// <param name="nodeKey">Node key to start at</param>
        /// <param name="lineIndex">Index to start execution on this node</param>
        ///  <exception cref="NodeNotFoundException">If there is no node found
        ///  with the specified key</exception>
        public void SetCurrentNode(string currentNode, int lineIndex = 0)
        {
            if (!nodes.ContainsKey(currentNode))
            {
                throw new NodeNotFoundException($"Node trying to set does not exist in the sequence. Create the node before setting the current node '{currentNode}'");
            }
            this.currentNode = currentNode;
            this.lineIndex = lineIndex;
        }

        /// <summary>
        /// Gets the currently visible line. A line is considered visible if
        /// it is an optional line or a dialogue line.
        /// </summary>
        /// <returns>Null if there is no currently visible line</returns>
        public Line GetCurrentVisibleLine()
        {
            if (currentNode == null)
                return null;

            if(nodes.TryGetValue(currentNode, out Node found))
            {
                return found.GetLine(lineIndex);
            }

            return null;
        }

        /// <summary>
        /// Adds a reader to read dialogue lines on this sequence
        /// </summary>
        /// <param name="reader">Reader to add</param>
        public void AddDialogueReader(IDialogueReader reader)
        {
            this.dialogueReaders.Add(reader);
        }

        /// <summary>
        /// Attempts to remove the function with the given key in this sequence
        /// <br></br>
        /// If the function has not been defined, this just returns false
        /// </summary>
        /// <param name="key">Key of function to remove</param>
        /// <returns>Whether the function was successfully removed</returns>
        public bool RemoveFunction(string key)
        {
            if (functionMap.ContainsKey(key))
            {
                functionMap.Remove(key);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the function with the given key to the given delegate
        /// </summary>
        /// <param name="key">Key of the function</param>
        /// <param name="function">Actual invoked function on sequence</param>
        /// <exception cref="ArgumentException">Thrown if the function has
        /// already been defined</exception>
        public void SetFunction(string key, Delegate function)
        {
            if (functionMap.ContainsKey(key))
            {
                throw new ArgumentException($"Function with key {key} already exists");
            }

            functionMap.Add(key, function);
        }

        /// <summary>
        /// Updates an existing function or adds it if it doesn't exist
        /// </summary>
        /// <param name="key">Key of the function</param>
        /// <param name="function">New function to use</param>
        public void UpdateFunction(string key, Delegate function)
        {
            if (functionMap.ContainsKey(key))
            {
                functionMap[key] = function;
            }
            else
            {
                functionMap.Add(key, function);
            }
        }

        /// <summary>
        /// Invokes the function with the given key and passed parameters
        /// </summary>
        /// <param name="key">Key of function</param>
        /// <param name="parameters">Parameters to pass onto function</param>
        /// <exception cref="ArgumentException">Thrown If the function does not
        /// exist with the given key in the sequence</exception>
        public void InvokeFunction(string key, params string[] parameters)
        {
            if (!functionMap.TryGetValue(key, out Delegate function))
            {
                throw new ArgumentException($"Key {key} does not have a function");
            }

            // just throw exceptions if errors out

            function.DynamicInvoke(parameters);
        }

        /// <summary>
        /// Gets the optional line containing the option containing with the 
        /// given ID. If no optional line exists, returns null
        /// </summary>
        /// <param name="id">id to search for</param>
        /// <returns>The line with the ID</returns>
        /// <exception cref="ArgumentException">Thrown if the types do not 
        /// match or no element present with given ID</exception>
        public T GetLineWithID<T>(string id) where T : Line
        {
            if (idLines.TryGetValue(id, out Line line))
            {
                if (line is not T casted)
                {
                    throw new ArgumentException("Type of given line does not match");
                }

                return casted;
            }

            throw new ArgumentException($"Element with ID {id} does not exist");
        }
    }
}