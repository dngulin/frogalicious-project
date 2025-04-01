using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Frog.Localization.Editor
{
    public class CodeScrapper
    {
        public static void Run(Dictionary<string, TranslationUsage> entries)
        {
            var reports = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(AbstractTrReport)))
                .Select(type => (AbstractTrReport)Activator.CreateInstance(type));

            var assetsPath = Application.dataPath + "/";

            foreach (var report in reports)
            foreach (var repEntry in report.Entries)
            {
                var path = repEntry.FullPath.StartsWith(assetsPath)
                    ? repEntry.FullPath[assetsPath.Length..]
                    : repEntry.FullPath;
                var source = $"{path}:{repEntry.LineNumber}";

                if (entries.TryGetValue(repEntry.MsgId, out var locEntry))
                {
                    locEntry.Sources.Add(source);
                    Debug.Assert(repEntry.IsPlural == locEntry.IsPlural);
                }
                else
                {
                    locEntry = new TranslationUsage(repEntry.MsgId, repEntry.IsPlural);
                    locEntry.Sources.Add(source);
                    entries.Add(repEntry.MsgId, locEntry);
                }
            }
        }
    }
}