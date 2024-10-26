using System;
using System.IO;

namespace Frog.ProtoPuff.Editor
{
    internal readonly struct BracesScope : IDisposable
    {
        private const string Indent = "    ";

        private readonly StreamWriter _writer;
        private readonly int _depth;

        public BracesScope(StreamWriter writer, int depth = 0)
        {
            _writer = writer;
            _depth = depth;

            WriteIndent(-1);
            _writer.WriteLine("{");
        }

        public void Dispose()
        {
            WriteIndent(-1);
            _writer.WriteLine("}");
        }

        private void WriteIndent(int shift = 0)
        {
            for (var i = 0; i < _depth + shift; i++)
            {
                _writer.Write(Indent);
            }
        }

        public void WriteLine(string value = "")
        {
            if (value != "")
                WriteIndent();

            _writer.WriteLine(value);
        }

        public BracesScope Braces(string prefix)
        {
            WriteIndent();
            _writer.WriteLine(prefix);

            return new BracesScope(_writer, _depth + 1);
        }
    }

    internal static class StreamWriterExtensions
    {
        public static BracesScope Braces(this StreamWriter w, string prefix)
        {
            w.WriteLine(prefix);
            return new BracesScope(w, 1);
        }
    }
}