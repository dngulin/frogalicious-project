using System.Collections.Generic;

namespace Frog.Localization.Editor.Gettext
{
    public struct PoEntry
    {
        public string TranslationId;
        public string EngStr;
        public string EngStrPlural;
        public List<string> Translations;

        public readonly bool IsValid => EngStr != null && Translations != null;
        public readonly bool HasData => TranslationId != null || EngStr != null || EngStrPlural != null || Translations != null;
    }
}