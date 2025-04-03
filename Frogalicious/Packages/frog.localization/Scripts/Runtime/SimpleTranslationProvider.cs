using System;
using System.Collections.Generic;
using System.Linq;

namespace Frog.Localization
{
    [Serializable]
    public struct Translation
    {
        public readonly string Id;
        public readonly string Value;
        public readonly int PluralForm;

        public Translation(string id, string value, int pluralForm)
        {
            Id = id;
            Value = value;
            PluralForm = pluralForm;
        }
    }

    public abstract class SimpleTranslationProvider : TranslationProvider
    {
        private readonly Dictionary<(string, int), string> _translations;

        protected SimpleTranslationProvider(IEnumerable<Translation> translations)
        {
            _translations = translations.ToDictionary(t => (t.Id, t.PluralForm), t => t.Value);
        }

        protected abstract int GetPluralForm(int count);

        public override string GetString(string id)
        {
            return _translations.GetValueOrDefault((id, 0), id);
        }

        public override string GetPlural(string id, int count)
        {
            var form = GetPluralForm(count);
            return _translations.GetValueOrDefault((id, form), id);
        }
    }
}