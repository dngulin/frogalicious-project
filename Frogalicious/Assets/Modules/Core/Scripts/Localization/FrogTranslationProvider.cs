using System;
using System.Collections.Generic;
using Frog.Localization;

namespace Frog.Core.Localization
{
    public readonly struct TrKey
    {
        public readonly uint TrIdHash;
        public readonly byte PluralForm;

        public TrKey(uint trIdHash, byte pluralForm)
        {
            TrIdHash = trIdHash;
            PluralForm = pluralForm;
        }

        public TrKey(string trId, byte pluralForm)
        {
            TrIdHash = trId.XxHash32();
            PluralForm = pluralForm;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TrIdHash, PluralForm);
        }
    }

    public class FrogTranslationProvider : TranslationProvider
    {
        private readonly Dictionary<TrKey, string> _translations;
        private readonly Func<int, int> _getPluralForm;

        public FrogTranslationProvider(Dictionary<TrKey, string> translations, Func<int, int> getPluralForm)
        {
            _translations = translations;
            _getPluralForm = getPluralForm;
        }

        public override string GetString(string id)
        {
            return _translations.GetValueOrDefault(new TrKey(id, 0), id);
        }

        public override string GetPlural(string id, int count)
        {
            var form = (byte)_getPluralForm(count);
            return _translations.GetValueOrDefault(new TrKey(id, form), id);
        }
    }
}