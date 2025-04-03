using System.Collections.Generic;
using System.IO;

namespace Frog.Gettext
{
    public static class PoFileReader
    {
        public static IEnumerable<PoEntry> Open(string path)
        {
            var state = new PoParserState();

            foreach (var line in File.ReadLines(path))
            {
                state.LineNumber++;

                if (string.IsNullOrWhiteSpace(line))
                {
                    if (state.TryEmitEntry(out var entry))
                        yield return entry;

                    continue;
                }

                if (line.StartsWith("#"))
                {
                    ParseComment(state, line);
                    continue;
                }

                ParseValue(state, line);
            }

            {
                if (state.TryEmitEntry(out var entry))
                    yield return entry;
            }
        }

        private static void ParseComment(PoParserState state, string line)
        {

            if (line.StartsWith("#  "))
            {
                state.AddTranslatorsComment(line[3..]);
                return;
            }

            if (line.StartsWith("#. "))
            {
                state.AddExtractedComment(line[3..]);
                return;
            }

            if (line.StartsWith("#: "))
            {
                state.AddReference(line[3..]);
                return;
            }

            if (line.StartsWith("#, "))
            {
                state.AddFlag(line[3..]);
            }
        }

        private static void ParseValue(PoParserState state, string line)
        {
            var valBeginIncl = line.IndexOf('"') + 1;
            var valEndExcl = line.LastIndexOf('"');

            if (valBeginIncl <= 0 || valEndExcl < 0)
                throw new PoParserException(state.LineNumber);

            var strValue = Unescape(line[valBeginIncl..valEndExcl]);

            if (line.StartsWith("\""))
            {
                state.UpdateCurrentString(strValue);
                return;
            }

            if (line.StartsWith("msgctx"))
            {
                state.StartString(ParsingStrType.MsgCtx, strValue);
                return;
            }

            if (line.StartsWith("msgid_plural"))
            {
                state.StartString(ParsingStrType.MsgIdPlural, strValue);
                return;
            }

            if (line.StartsWith("msgid"))
            {
                state.StartString(ParsingStrType.MsgId, strValue);
                return;
            }

            if (line.StartsWith("msgstr"))
            {
                state.StartString(ParsingStrType.MsgStr, strValue);
                return;
            }

            throw new PoParserException(state.LineNumber);
        }

        private static string Unescape(string str)
        {
            return str.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
        }
    }
}