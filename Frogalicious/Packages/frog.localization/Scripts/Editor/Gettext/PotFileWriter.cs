using System;
using System.IO;

namespace Frog.Localization.Editor.Gettext
{
    public class PotFileWriter : IDisposable
    {
        private readonly StreamWriter _writer;

        public PotFileWriter(string path)
        {
            _writer = new StreamWriter(path);
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public void Write(in PotEntry entry)
        {
            _writer.WriteLine(PoConventions.TrIdCommentPrefix + entry.TranslationId);

            foreach (var src in entry.Sources)
                _writer.WriteLine("#: " + src);

            _writer.WriteLine("#, csharp-format");

            _writer.WriteLine("msgid \"" + Escape(entry.EngStr) + "\"");
            if (entry.IsPlural)
                _writer.WriteLine("msgid_plural \"" + Escape(entry.EngStrPlural) + "\"");

            if (entry.IsPlural)
            {
                _writer.WriteLine("msgstr[0] \"\"");
                _writer.WriteLine("msgstr[1] \"\"");
            }
            else
            {
                _writer.WriteLine("msgstr \"\"");
            }

            _writer.WriteLine();
        }

        private static string Escape(string str)
        {
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        }
    }
}