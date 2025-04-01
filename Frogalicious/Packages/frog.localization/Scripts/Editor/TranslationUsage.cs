using System.Collections.Generic;

namespace Frog.Localization.Editor
{
    public readonly struct TranslationUsage
    {
        public readonly string MsgId;
        public readonly bool IsPlural;
        public readonly List<string> Sources;

        public TranslationUsage(string msgId, bool isPlural)
        {
            MsgId = msgId;
            IsPlural = isPlural;
            Sources = new List<string>();
        }
    }
}