using System;
using System.IO;

namespace Frog.Gettext
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
                _writer.WriteLine("msgctx \"" + entry.OptContext.Replace("\n", "#: "));

            _writer.WriteLine("msgid \"" + Escape(entry.EngStr) + "\"");

            if (entry.IsPlural)
                _writer.WriteLine("msgid_plural \"" + Escape(entry.OptEngStrPlural) + "\"");

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