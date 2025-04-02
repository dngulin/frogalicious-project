using System;

namespace Frog.Localization.Editor.Gettext
{
    public class PoParserException : Exception
    {
        public PoParserException(int lineNumber) : base($"Unexpected value at line {lineNumber}")
        {
        }
    }
}