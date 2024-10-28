namespace Frog.ProtoPuff.Editor.Lexer
{
    internal struct Token
    {
        public TokenType Type;
        public Location Location;
        public string OptValue;
    }

    public enum TokenType
    {
        Identifier,
        Number,
        Colon,
        Semicolon,
        Assignment,
        CurlyBraceLeft,
        CurlyBraceRight,
    }
}