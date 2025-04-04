using System;
using System.IO;

namespace Frog.Gettext
{
    public class PoFileWriter : IDisposable
    {
        private readonly StreamWriter _writer;

        public PoFileWriter(string path)
        {
            _writer = new StreamWriter(path);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Write(in PoEntry entry)
        {
            foreach (var comment in entry.TranslatorsComments)
                _writer.WriteLine("#  " + comment.Replace("\n", "#  "));

            foreach (var comment in entry.ExtractedComments)
                _writer.WriteLine("#. " + comment.Replace("\n", "#. "));

            foreach (var reference in entry.References)
                _writer.WriteLine("#: " + reference.Replace("\n", "#: "));

            foreach (var flag in entry.Flags)
                _writer.WriteLine("#, " + flag.Replace("\n", "#, "));

            if (entry.OptContext != null)
                WriteEscaped("msgctx", entry.OptContext);

            WriteEscaped("msgid", entry.EngStr);

            if (entry.OptEngStrPlural != null)
                WriteEscaped("msgid_plural", entry.OptEngStrPlural);

            if (entry.Translations.Count == 1)
            {
                WriteEscaped("msgstr", entry.Translations[0]);
            }
            else
            {
                for (var i = 0; i < entry.Translations.Count; i++)
                {
                    _writer.Write("msgstr[");
                    _writer.Write(i);
                    _writer.Write("] \"");
                    _writer.Write(Escape(entry.Translations[i]));
                    _writer.WriteLine("\"");
                }

            }

            _writer.WriteLine();
        }

        private void WriteEscaped(string key, string value)
        {
            _writer.Write(key);
            _writer.Write(" \"");
            _writer.Write(Escape(value));
            _writer.WriteLine("\"");
        }

        private static string Escape(string str)
        {
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        }
    }
}