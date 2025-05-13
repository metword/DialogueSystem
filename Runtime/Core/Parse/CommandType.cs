/// <summary>
/// Unique special command types that can exist in the dialogue parse
/// </summary>
public enum CommandType
{
    Dialogue,
    Option,
    NodeStart,
    NodeEnd,
    Exec,
    Goto,
    Comment,
    Escape,
    IdStart,
    IdEnd,
    FunctionStart,
    FunctionEnd,
    ParamDelim,
    FormatStart,
    FormatEnd,
}
