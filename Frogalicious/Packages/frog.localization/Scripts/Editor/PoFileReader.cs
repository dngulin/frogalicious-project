using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Frog.Localization.Editor
{
    public struct PoEntry
    {
        public string EngStr;
        public string EngStrPlural;
        public List<string> Translations;
        public string TranslationId;

        public readonly bool IsValid => EngStr != null && Translations != null;
        public readonly bool HasData => EngStr != null || EngStrPlural != null || Translations != null || TranslationId != null;
    }

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
                else if (line.StartsWith("msgid"))
                {
                    state.StartString(ParsingStrType.MsgId, strValue);
                }
                else if (line.StartsWith("msgid_plural"))
                {
                    state.StartString(ParsingStrType.MsgIdPlural, strValue);
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

    public class PoParserException : Exception
    {
        public PoParserException(int lineNumber) : base($"Unexpected value at line {lineNumber}")
        {
        }
    }

    internal enum ParsingStrType
    {
        MsgId,
        MsgIdPlural,
        MsgStr
    }

    internal class PoParserState
    {
        public int LineNumber;

        private PoEntry _entry;
        private ParsingStrType? _currStrType;
        private string _currString;

        public bool TryEmitEntry(out PoEntry entry)
        {
            FinalizeCurrStr();
            if (_entry.IsValid)
            {
                entry = _entry;
                _entry = default;
                return true;
            }

            if (_entry.HasData)
                throw new PoParserException(LineNumber);

            entry = default;
            return false;
        }

        private void FinalizeCurrStr()
        {
            switch (_currStrType)
            {
                case null:
                    break;
                case ParsingStrType.MsgId:
                    _entry.EngStr = _currString;
                    break;
                case ParsingStrType.MsgIdPlural:
                    _entry.EngStrPlural = _currString;
                    break;
                case ParsingStrType.MsgStr:
                    _entry.Translations ??= new List<string>(1);
                    _entry.Translations.Add(_currString);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _currString = null;
            _currStrType = null;
        }

        public void StartString(ParsingStrType type, string value)
        {
            FinalizeCurrStr();
            _currStrType = type;
            _currString = value;
        }

        public void UpdateCurrentString(string value)
        {
            if (_currStrType == null)
                throw new PoParserException(LineNumber);

            Debug.Assert(_currString != null);
            _currString += value;
        }

        public void SetTranslationId(string id)
        {
            if (_entry.TranslationId != null)
                throw new PoParserException(LineNumber);

            _entry.TranslationId = id;
        }
    }
}