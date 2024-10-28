namespace Frog.ProtoPuff.Editor.Parser
{
    internal struct ParserState
    {
        public ParsingTypeKind ParsingTypeKind;
    }

    internal enum ParsingTypeKind
    {
        None,
        Enum,
        Struct,
    }
}