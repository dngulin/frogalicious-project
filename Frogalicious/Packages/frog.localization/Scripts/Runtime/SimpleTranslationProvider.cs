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

    public sealed class SimpleTranslationProvider : TranslationProvider
    {
        private readonly Dictionary<(string, int), string> _translations;
        private readonly Func<int, int> _getPluralForm;

        public SimpleTranslationProvider(IEnumerable<Translation> translations, Func<int, int> getPluralForm)
        {

            _translations = translations.ToDictionary(t => (t.Id, t.PluralForm), t => t.Value);
            _getPluralForm = getPluralForm;
        }

        public override string GetString(string id)
        {
            return _translations.GetValueOrDefault((id, 0), id);
        }

        public override string GetPlural(string id, int count)
        {
            var form = _getPluralForm(count);
            return _translations.GetValueOrDefault((id, form), id);
        }
    }
}