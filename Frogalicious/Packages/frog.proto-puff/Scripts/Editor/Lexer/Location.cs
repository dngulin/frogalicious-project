namespace Frog.ProtoPuff.Editor.Lexer
{
    internal struct Location
    {
        public int Line;
        public int Column;

        public override string ToString() => $"{Line + 1}:{Column + 1}";
    }
}