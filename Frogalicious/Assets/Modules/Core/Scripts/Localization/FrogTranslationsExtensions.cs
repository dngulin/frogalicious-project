using System.Collections.Generic;
using Frog.Collections;
using Frog.Core.Save;

namespace Frog.Core.Localization
{
    public static class FrogTranslationsExtensions
    {
        public static void Add(this ref FrogTranslations self, string id, byte form, string value)
        {
            ref var entry = ref self.Entries.RefAdd();
            entry.TranslationIdHash = id.XxHash32();
            entry.PluralForm = form;
            entry.Translation.AppendUtf8String(value);
        }

        public static Dictionary<TrKey, string> ToDictionary(this in FrogTranslations self)
        {
            var dict = new Dictionary<TrKey, string>(self.Entries.Count());

            foreach (ref readonly var entry in self.Entries.RefReadonlyIter())
            {
                var key = new TrKey(entry.TranslationIdHash, entry.PluralForm);
                var val = entry.Translation.ToStringUtf8();
                dict.Add(key, val);
            }

            return dict;
        }
    }
}