using System.Collections.Generic;
using System.IO;

namespace Frog.Localization.Editor.Gettext
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

                const string translationIdPrefix = PoConventions.TrIdCommentPrefix;
                if (line.StartsWith(translationIdPrefix))
                {
                    state.SetTranslationId(line[translationIdPrefix.Length..]);
                    continue;
                }

                if (line.StartsWith("#"))
                    continue;

                var valBeginIncl = line.IndexOf('"') + 1;
                var valEndExcl = line.LastIndexOf('"');

                if (valBeginIncl <= 0 || valEndExcl < 0)
                {
                    throw new PoParserException(state.LineNumber);
                }

                var strValue = Unescape(line[valBeginIncl..valEndExcl]);

                if (line.StartsWith("\""))
                {
                    state.UpdateCurrentString(strValue);
                }
                else if (line.StartsWith("msgid_plural"))
                {
                    state.StartString(ParsingStrType.MsgIdPlural, strValue);
                }
                else if (line.StartsWith("msgid"))
                {
                    state.StartString(ParsingStrType.MsgId, strValue);
                }
                else if (line.StartsWith("msgstr"))
                {
                    state.StartString(ParsingStrType.MsgStr, strValue);
                }
                else
                {
                    throw new PoParserException(state.LineNumber);
                }
            }

            {
                if (state.TryEmitEntry(out var entry))
                    yield return entry;
            }
        }

        private static string Unescape(string str)
        {
            return str.Replace("\\n", "\n").Replace("\\\"", "\"").Replace("\\\\", "\\");
        }
    }
}