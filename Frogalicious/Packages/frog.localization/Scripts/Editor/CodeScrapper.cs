using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Frog.Localization.Editor
{
    public static class CodeScrapper
    {
        public static void Run(Dictionary<string, TranslationUsage> usages)
        {
            var reports = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsSubclassOf(typeof(AbstractTrReport)))
                .Select(type => (AbstractTrReport)Activator.CreateInstance(type));

            var assetsPath = Application.dataPath + "/";

            foreach (var report in reports)
            foreach (var entry in report.Entries)
            {
                var path = entry.FullPath.StartsWith(assetsPath)
                    ? entry.FullPath[assetsPath.Length..]
                    : entry.FullPath;
                var source = $"{path}:{entry.LineNumber}";

                usages.Add(entry.TranslationId, source, entry.IsPlural);
            }
        }
    }
}