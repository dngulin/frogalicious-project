using System.Collections.Generic;
using UnityEngine;

namespace Frog.Localization.Editor
{
    public static class TrUsagesExtensions
    {
        public static void Add(this Dictionary<string, TranslationUsage> self, string id, string src, bool plural)
        {
            if (self.TryGetValue(id, out var usage))
            {
                usage.Sources.Add(src);
                Debug.Assert(usage.IsPlural == plural);
            }
            else
            {
                usage = new TranslationUsage(id, plural);
                usage.Sources.Add(src);
                self.Add(id, usage);
            }
        }
    }
}