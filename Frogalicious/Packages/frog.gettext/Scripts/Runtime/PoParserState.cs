using System;
using UnityEngine;

namespace Frog.Gettext
{
    internal enum ParsingStrType
    {
        MsgCtx,
        MsgId,
        MsgIdPlural,
        MsgStr
    }

    internal class PoParserState
    {
        public int LineNumber;

        private PoEntry _entry = new PoEntry();
        private ParsingStrType? _currStrType;
        private string _currString;

        public bool TryEmitEntry(out PoEntry entry)
        {
            FinalizeCurrStr();
            if (_entry.IsValid)
            {
                entry = _entry;
                _entry = new PoEntry();
                return true;
            }

            if (_entry.HasTranslationData)
                throw new PoParserException(LineNumber);

            entry = null;
            return false;
        }

        private void FinalizeCurrStr()
        {
            switch (_currStrType)
            {
                case null:
                    break;
                case ParsingStrType.MsgCtx:
                    if (_entry.OptContext != null) throw new PoParserException(LineNumber);
                    _entry.OptContext = _currString;
                    break;
                case ParsingStrType.MsgId:
                    if (_entry.EngStr != null) throw new PoParserException(LineNumber);
                    _entry.EngStr = _currString;
                    break;
                case ParsingStrType.MsgIdPlural:
                    if (_entry.EngStr != null) throw new PoParserException(LineNumber);
                    _entry.OptEngStrPlural = _currString;
                    break;
                case ParsingStrType.MsgStr:
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

        public void AddTranslatorsComment(string comment) => _entry.TranslatorsComments.Add(comment);
        public void AddExtractedComment(string comment) => _entry.ExtractedComments.Add(comment);
        public void AddReference(string comment) => _entry.References.Add(comment);
        public void AddFlag(string flag) => _entry.Flags.Add(flag);
    }
}