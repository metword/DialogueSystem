using System;
using System.Collections.Generic;
using System.Text;

namespace DialogueSystem
{
    /// <summary>
    /// An optional line presents the IDialogueReader with a set of options 
    /// to display. Each individual option is identified with an integer key
    /// </summary>
    public class OptionalLine : Line
    {
        private Dictionary<int, Option> options;

        /// <summary>
        /// Construct a new optional line
        /// </summary>
        public OptionalLine()
        {
            this.options = new();
        }

        /// <summary>
        /// Adds an option to the optional line
        /// </summary>
        /// <param name="node">node to change control flow to if chosen</param>
        /// <param name="option">Text present in the option</param>
        /// <returns>The ID of the newly created option</returns>
        public int AddOption(string node, string option)
        {
            Option newOption = new(node, option);

            // just put options at the first available index
            int id = options.Count;

            options.Add(id, newOption);

            return id;
        }

        /// <summary>
        /// Gets the option in this optional line with the given ID
        /// <br></br>
        /// Ids are numbered starting at 0 to Options.Count
        /// </summary>
        /// <param name="id">Id of option to get</param>
        /// <returns>Returns the option if it exist</returns>
        /// <exception cref="ArgumentException">Thrown if the id does not exist
        /// on this OptionalLine</exception>
        public Option GetOption(int id)
        {
            if (options.TryGetValue(id, out Option returned))
            {
                return returned;
            }

            throw new ArgumentException("Options does not contain option with given ID");
        }

        /// <summary>
        /// Attempts to ge the option in this optional line with the given ID.
        /// </summary>
        /// <param name="id">Id to get</param>
        /// <param name="option">returned option if it exists</param>
        /// <returns>true if the option was obtained, else false</returns>
        public bool TryGetOption(int id, out Option option)
        {
            if (options.TryGetValue(id, out Option returned))
            {
                option = returned;
                return true;
            }

            option = null;
            return false;
        }

        public int CountOptions()
        {
            return this.options.Count;
        }

        /// <summary>
        /// Get all the options in this OptionalLine
        /// </summary>
        /// <returns>All present options</returns>
        public Dictionary<int, Option> GetOptions()
        {
            return options;
        }

        /// <summary>
        /// Gets the options that have been enabled within this line
        /// </summary>
        /// <returns></returns>
        public List<Option> GetEnabledOptions()
        {
            List<Option> list = new List<Option>();
            foreach (Option option in options.Values)
            {
                if (option.Enabled)
                    list.Add(option);
            }
            return list;
        }

        /// <summary>
        /// Gets the destination node for the selected ID
        /// </summary>
        /// <param name="id">The id of the option being selected</param>
        /// <returns>The target node</returns>
        /// <exception cref="ArgumentException">Thrown if the id is not present
        /// in the dictionary</exception>
        public string GetNodeFromSelection(int id)
        {
            if (!options.ContainsKey(id))
            {
                throw new ArgumentException($"ID {id} not present in options");
            }

            Option option = options[id];
            return option.Node;
        }

        /// <summary>
        /// Toggles an option that has been identitified using an option ID
        /// </summary>
        /// <param name="id">Id present in the dictionary</param>
        /// <param name="enabled">enable or disable the line?</param>
        public void ToggleOption(int id, bool enabled)
        {
            if (options.TryGetValue(id, out Option option))
            {
                option.Enabled = enabled;
            }
        }

        /// <summary>
        /// Toggles an option that has been identified using a string 
        /// identifier that has been placed onto the option.
        /// </summary>
        /// <param name="stringId">Id of the option</param>
        /// <param name="enabled">enable or disable the line?</param>
        public void ToggleOption(string stringId, bool enabled)
        {
            foreach (Option option in options.Values)
            {
                if (option.Id == stringId)
                {
                    option.Enabled = enabled;
                }
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new();
            builder.Append("OptionalLine");
            foreach (Option opt in options.Values)
            {
                builder.Append("\n")
                    .Append(opt.ToString());
            }

            return builder.ToString();
        }
    }
}