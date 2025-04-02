using System;
using System.Collections.Generic;
using UnityEngine;

namespace Frog.Localization.Editor.Gettext
{
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
            Debug.Assert(_entry.TranslationId == null);
            _entry.TranslationId = id;
        }
    }
}