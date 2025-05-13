using DialogueSystem.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogueSystem
{
    /// <summary>
    /// The format parser takes input in the format of a string in the 
    /// formatting language and turns it into a string with Ids and the start
    /// and end demarcated
    /// </summary>
    public class FormatParser
    {
        /// <summary>
        /// Formats the given text into an identified string which contains
        /// data about ids and their positions within the string.
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>The built identified string</returns>
        public IdentifiedString Format(string text)
        {
            Dictionary<CommandType, Command> usedCommands = CommandSettings.formatCommands;
            Command escape = usedCommands[CommandType.Escape];
            List<Command> commands = usedCommands.Values.ToList();

            string parse = text;
            int parsePart = 0;
            StringBuilder output = new();

            // need to keep track of the indices that we'll have parsed
            int startId = -1;
            int endId = -1;
            List<(string, int, int)> ids = new();

            while (parse.Length > 0)
            {
                (string before, string after, string next, Command command) = StringUtils.SplitCommand(parse, escape, commands);

                // UnityEngine.Debug.Log($"Before: {before}, after: {after}, next: {next}, Command: " + command);

                // if no command just add all before to buffer and clear parse
                if (command == null)
                {
                    output.Append(before);
                    parse = "";
                }

                // we're parsing the $ if so
                else if (parsePart == 0)
                {
                    if (command.Type != CommandType.IdStart)
                    {
                        throw new FormatParseException("Ids must begin with a $");
                    }

                    output.Append(before);
                    parsePart++;
                    parse = after + next;

                    // UnityEngine.Debug.Log($"Before: <{before}> length={before.Length}");
                    // UnityEngine.Debug.Log($"Output: <{output.ToString()}> length={output.Length}");

                    startId = output.Length;
                }
                // we're parsing the (
                else if (parsePart == 1)
                {
                    if (command.Type != CommandType.FormatStart)
                    {
                        throw new FormatParseException("Id must be followed by an (");
                    }

                    output.Append(after);

                    // trim the ID
                    string id = before.Trim();

                    if (!StringUtils.IsOneWordWithoutSymbols(id))
                    {
                        throw new FormatParseException("Id must be one word with no symbols");
                    }

                    string idText = after;
                    endId = startId + idText.Length;
                    // UnityEngine.Debug.Log($"EndId={endId}");


                    // add the ID here
                    ids.Add((id, startId, endId));

                    parsePart++;
                    parse = next;
                }
                // we're parsing the )
                else if (parsePart == 2)
                {
                    if (command.Type != CommandType.FormatEnd)
                    {
                        throw new FormatParseException("Text must be closed with an )");
                    }

                    // just take text after the ) rather than using the split
                    parse = parse[command.CommandString.Length..];
                    parsePart = 0;
                }
            }

            if (parsePart != 0)
            {
                throw new FormatParseException($"Unfinished Id in format {parsePart}");
            }

            IdentifiedString identifiedString = new (output.ToString());

            foreach ((string id, int start, int end) in ids)
            {
                identifiedString.AddBounds(id, start, end);
            }

            return identifiedString;
        }
    }
}

