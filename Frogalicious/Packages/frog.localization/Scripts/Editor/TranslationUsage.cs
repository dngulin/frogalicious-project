using System.Collections.Generic;

namespace Frog.Localization.Editor
{
    public readonly struct TranslationUsage
    {
        public readonly string TranslationId;
        public readonly bool IsPlural;
        public readonly List<string> Sources;

        public TranslationUsage(string translationId, bool isPlural)
        {
            TranslationId = translationId;
            IsPlural = isPlural;
            Sources = new List<string>();
        }
    }
}