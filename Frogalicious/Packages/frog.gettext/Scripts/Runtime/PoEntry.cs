using System.Collections.Generic;

namespace Frog.Gettext
{
    public class PoEntry
    {
        /// <summary>
        /// Corresponds to `# ` comments
        /// </summary>
        public readonly List<string> TranslatorsComments = new List<string>();

        /// <summary>
        /// Corresponds to `#.` comments
        /// </summary>
        public readonly List<string> ExtractedComments = new List<string>();

        /// <summary>
        /// Corresponds to `#:` comments
        /// </summary>
        public readonly List<string> References = new List<string>();

        /// <summary>
        /// Corresponds to `#,` comment (comma-separated list)
        /// </summary>
        public readonly List<string> Flags = new List<string>();

        /// <summary>
        /// Corresponds to the `msgctx` value
        /// </summary>
        public string OptContext;

        /// <summary>
        /// Corresponds to the `msgid` value
        /// </summary>
        public string EngStr;

        /// <summary>
        /// Corresponds to the `msgid_plural` value
        /// </summary>
        public string OptEngStrPlural;

        /// <summary>
        /// Corresponds to the `msgstr` and `msgstr[N]` values
        /// </summary>
        public readonly List<string> Translations = new List<string>();

        public bool IsValid => EngStr != null && Translations.Count != 0;
        public bool HasTranslationData => EngStr != null || OptEngStrPlural != null || Translations.Count != 0;
    }
}