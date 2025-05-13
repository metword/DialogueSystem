using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    /// <summary>
    /// Gets the commands present in the Dialogue System
    /// </summary>
    public static class CommandSettings
    {
        private static readonly List<Command> definedCommands = new();

        private static readonly List<Command> defaultCommands = new()
        {
            new Command(CommandType.Dialogue, ":"),
            new Command(CommandType.Option, "->"),
            new Command(CommandType.NodeStart, "-"),
            new Command(CommandType.NodeEnd, "--"),
            new Command(CommandType.Exec, ">"),
            new Command(CommandType.Goto, "|"),
            new Command(CommandType.Comment, "#"),
            new Command(CommandType.Escape, "\\"),
            new Command(CommandType.IdStart, "["),
            new Command(CommandType.IdEnd, "]"),
        };

        private static readonly List<Command> functionParts = new()
        {
            new Command(CommandType.Escape, "\\"),
            new Command(CommandType.FunctionStart, "("),
            new Command(CommandType.FunctionEnd, ")"),
            new Command(CommandType.ParamDelim, ","),
        };

        private static readonly List<Indentation> indentations = new()
        {
            new Indentation(' ', 4),
            new Indentation('\t', 1),
        };

        // formatting is done like so $id(text)

        public static readonly Dictionary<CommandType, Command> formatCommands = new()
        {
            { CommandType.Escape, new Command(CommandType.Escape, "\\") },
            { CommandType.IdStart, new Command(CommandType.IdStart, "$") },
            { CommandType.FormatStart, new Command(CommandType.FormatStart, "(") },
            { CommandType.FormatEnd, new Command(CommandType.FormatEnd, ")") },
        };

        /// <summary>
        /// Defines a command to be parsed with a modified key and value beyond 
        /// the default outlined
        /// </summary>
        /// <param name="command">The command being defined</param>
        public static void DefineCommand(Command command)
        {
            definedCommands.Add(command);
        }

        /// <summary>
        /// Gets a sorted list of commands based on priority to parse using any
        /// user defined command characters.
        /// </summary>
        /// <returns>The sorted list of commands</returns>
        public static List<Command> GetCommands()
        {
            List<Command> commands = new(defaultCommands);

            // go through each of the defined commands and replace
            foreach (Command definedCommand in definedCommands)
            {
                CommandType definedType = definedCommand.Type;

                commands.RemoveAll(existing => existing.Type == definedType);

                commands.Add(definedCommand);
            }

            commands.Sort();
            return commands;
        }

        /// <summary>
        /// Gets the user defined indentations present in this settings
        /// </summary>
        /// <returns>The list</returns>
        public static List<Indentation> GetIndentations()
        {
            return indentations;
        }

        /// <summary>
        /// Gets the user defined function parts used to parse functions
        /// </summary>
        /// <returns>The dictionary</returns>
        public static List<Command> GetFunctionParts()
        {
            return functionParts;
        }

        /// <summary>
        /// Gets the command with the given idType within the whole list of commands
        /// </summary>
        /// <param name="idType">Id type to get</param>
        /// <returns>The found command or null if it is not present in the 
        /// defined command</returns>
        public static Command GetCommand(CommandType idType)
        {
            foreach (Command command in GetCommands())
            {
                if (command.Type == idType)
                    return command;
            }

            return null;
        }
    }
}
