using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DialogueSystem.Utils
{
    /// <summary>
    /// Various functions relating to strings useful to program function
    /// especially for tasks such as parsing.
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Splits the given text on the first instance of any command.
        /// The text is split into 3 parts, the text that exists before the
        /// command, the text that exists after the command, and the text that
        /// would exist after this command which would be split on another
        /// command character.
        /// <br></br>
        /// When the text is split. The command is removed but all whitespace
        /// and other characters are kept
        /// <br></br>
        /// Escapes are only handled in before and after. Any escapes in the 
        /// rest of the text are ignored.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="escape">Escape character to escape other commands
        /// </param>
        /// <param name="allCommands">The commands to search for</param>
        /// <returns>A tuple containing the text before the command, the text 
        /// after the command and the text to be parsed in the next command. 
        /// <br></br>
        /// If there is no instance of the command in the text, everything is
        /// returned in before with empty strings for after and next.
        /// <br></br>
        /// If there is no next command, an empty string is returned for next.
        /// <br></br>
        /// In general, contents that don't exist / are not parsed, are set to
        /// empty strings rather than null.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if given a null text
        /// </exception>
        /// <exception cref="StringUtilsException">Thrown if attempting to
        /// split an invalid piece of dialogue (Unknown escape) </exception>
        public static (string before, string after, string next, Command command) SplitCommand(this string text, Command escape, List<Command> allCommands)
        {
            // can't be null
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text), "null text parameter cannot be split");
            }

            // add the escape if not present in all commands
            List<Command> searchCommands = new(allCommands);
            if (!allCommands.Contains(escape))
            {
                searchCommands.Add(escape);
            }

            List<StringBuilder> builders = new()
            {
                new(), // before
                new(), // after
                new(), // next
            };
            Command splittingCommand = null;

            int stringBuilderIndex = 0;
            int index = 0;
            while (index < text.Length)
            {
                StringBuilder currentBuilder = builders[stringBuilderIndex];

                string split = text[index..];
                Command found = GetStartCommand(split, searchCommands);

                if (found != null)
                {
                    if (found == escape)
                    {
                        int escapeLength = escape.CommandString.Length;
                        string afterEscape = split[escapeLength..];
                        Command escapedCommand = GetStartCommand(afterEscape, searchCommands);

                        if (escapedCommand == null)
                            throw new StringUtilsException($"Unknown escape character: {split}");

                        string escapedString = escapedCommand.CommandString;


                        // go past the escape and past the subsequent command
                        index += escapeLength + escapedString.Length;

                        // add to the builder
                        currentBuilder.Append(escapedString);
                    }
                    else
                    {
                        // go past command if first one
                        if (stringBuilderIndex == 0)
                        {
                            splittingCommand ??= found;
                            index += found.CommandString.Length;
                        } 
                        // add the whole rest if second time
                        else if (stringBuilderIndex == 1)
                        {
                            builders[stringBuilderIndex + 1].Append(split);
                            index = text.Length;
                        }

                        // change the builder
                        stringBuilderIndex++;
                    }
                }
                else
                {
                    currentBuilder.Append(text[index]);
                    index++;
                }
            }

            return (builders[0].ToString(), builders[1].ToString(), builders[2].ToString(), splittingCommand);
        }

        private static Command GetStartCommand(string text, List<Command> commands)
        {
            foreach (Command command in commands)
            {
                if (text.StartsWith(command.CommandString))
                {
                    return command;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns whether the given text is one word and does not contain any
        /// other symbols or text within
        /// </summary>
        /// <param name="text">Text to parse through</param>
        /// <returns>Whether it is a word without symbols or not</returns>
        public static bool IsOneWordWithoutSymbols(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            // Check if the string contains any whitespace characters.
            if (text.Any(char.IsWhiteSpace))
            {
                return false;
            }

            // Check if the string contains any symbols (non-alphanumeric characters).
            if (text.Any(c => !char.IsLetterOrDigit(c)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Finds the first index of the given substring starting at startIndex
        /// and going backwards in the string. The entire occurrence must be
        /// before the index to be captured.
        /// </summary>
        /// <param name="text">Text to search through</param>
        /// <param name="search">Text to search for</param>
        /// <param name="startIndex">Index to start search from</param>
        /// <returns>the index if found, else returns -1</returns>
        public static int IndexOfBefore(this string text, string search, int startIndex)
        {
            if (string.IsNullOrEmpty(search))
            {
                return startIndex;
            }

            // bound the index to 0 - text.Length
            if (startIndex > text.Length)
            {
                startIndex = text.Length;
            }

            for (int i = startIndex - 1; i >= 0; i--)
            {
                string sub = text[i.. startIndex];

                if (sub.StartsWith(search))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Finds the first index of the given substring starting at startIndex
        /// and going forwards in the string. The entire occurrence must be
        /// after the index to be captured.
        /// </summary>
        /// <param name="text">Text to search through</param>
        /// <param name="search">Text to search for</param>
        /// <param name="startIndex">Index to start search from</param>
        /// <returns>the index if found, else returns -1</returns>
        public static int IndexOfAfter(this string text, string search, int startIndex)
        {
            if (string.IsNullOrEmpty(search))
            {
                return startIndex;
            }
            // bound the index to 0 - text.Length
            if (startIndex < -1)
            {
                startIndex = -1;
            }

            for (int i = startIndex + 1; i < text.Length; i++)
            {
                string sub = text[i..];
                if (sub.StartsWith(search))
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns a substring without caring about bounds being out of bounds
        /// </summary>
        /// <param name="str">String to get substring of</param>
        /// <param name="start">Start index of substring (inclusive)</param>
        /// <param name="endIndex">End index of substring (exclusive)</param>
        /// <returns>The created</returns>
        public static string UnboundedSubstring(this string str, int start, int endIndex)
        {
            if (start < 0)
                start = 0;

            if (endIndex > str.Length)
                endIndex = str.Length;

            return str[start..endIndex];
        }
    }
}
