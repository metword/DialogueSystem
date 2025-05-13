using System;

/// <summary>
/// Unique commands that exist in the Dialogue Script text file
/// </summary>
public class Command : IComparable<Command>
{
    public CommandType Type { get; private set; }
    public string CommandString { get; private set; }

    /// <summary>
    /// Contruct a new command with given type on command string
    /// </summary>
    /// <param name="type">The type that this command is executing</param>
    /// <param name="commandString">String that is parsed for this command
    /// </param>
    public Command(CommandType type, string commandString)
    {
        Type = type;
        CommandString = commandString;
    }

    /// <summary>
    /// Commparse two commands. Command are considered higher priority if they
    /// have a longer string than the other commnad. If the commands have the
    /// same priority based on length, they compare strings for order
    /// </summary>
    /// <param name="other">Command to compare to</param>
    /// <returns>negative if this command is higher priority, 0 if equal prio
    /// positive if lesser priority</returns>
    public int CompareTo(Command other)
    {
        int compare = other.CommandString.Length - this.CommandString.Length;
        if (compare != 0)
        {
            return compare;
        }
        return this.CommandString.CompareTo(other.CommandString);

    }

    public override string ToString()
    {
        return $"Command {CommandString} {Type}";
    }
}
