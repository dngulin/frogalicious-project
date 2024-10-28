namespace Frog.ProtoPuff.Editor.Lexer
{
    internal struct LexerState
    {
        public LexerBlock CurrBlock;
        public Location CurrBlockLocation;
        public Location CurrLocation;
    }

    internal enum LexerBlock
    {
        None,
        Commentary,
        NameIdentifier,
        Number,
    }
}