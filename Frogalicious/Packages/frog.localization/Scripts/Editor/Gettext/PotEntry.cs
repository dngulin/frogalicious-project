using System.Collections.Generic;

namespace Frog.Localization.Editor.Gettext
{
    public struct PotEntry
    {
        public readonly string TranslationId;
        public readonly List<string> Sources;
        public readonly string EngStr;
        public readonly string EngStrPlural;

        private PotEntry(string translationId, List<string> sources, string engStr, string engStrPlural)
        {
            TranslationId = translationId;
            Sources = sources;
            EngStr = engStr;
            EngStrPlural = engStrPlural;
        }

        public readonly bool IsPlural => EngStrPlural != null;

        public static PotEntry Singular(string trId, List<string> sources, string engStr)
        {
            return new PotEntry(trId, sources, engStr, null);
        }

        public static PotEntry Plural(string trId, List<string> sources, string engStr, string engStrPlural)
        {
            return new PotEntry(trId, sources, engStr, engStrPlural);
        }
    }
}