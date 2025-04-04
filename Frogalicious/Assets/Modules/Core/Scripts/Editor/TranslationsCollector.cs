using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Frog.Gettext;
using Frog.Localization.Editor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Frog.Core.Editor
{
    public static class TranslationsCollector
    {
        private const string TrIdPrefix = "Translation id: ";

        [MenuItem("Tools/Localization/Build English Translation & Export POT-File", priority = 20)]
        public static void BuildEnglishTranslation()
        {
            var usages = new Dictionary<string, TrUsage>();
            TrUsagesByCode.AppendTo(usages);
            TrUsagesByStaticTextLocalizer.AppendTo(usages);

            var eng = LoadEnglishTranslationMap();
            foreach (var trId in eng.Keys)
            {
                if (!usages.ContainsKey(trId))
                    Debug.LogWarning($"Translation `{trId}` is defined but is not used");
            }

            var usagesList = usages.Values.ToList();
            usagesList.Sort((a, b) => string.Compare(a.TranslationId, b.TranslationId, StringComparison.Ordinal));

            using var writer = new PoFileWriter(Application.dataPath + "/../../localization/frog.pot");
            foreach (var usage in usagesList)
            {
                if (!eng.TryGetValue(usage.TranslationId, out var trForms))
                {
                    Debug.LogError($"Translation `{usage.TranslationId}` is not defined");
                    continue;
                }

                if (trForms.Length < 1 || trForms.Length > 2)
                {
                    Debug.LogError($"Translation `{usage.TranslationId}` definition is invalid ({trForms.Length} forms)");
                    continue;
                }

                var plural = trForms.Length == 2;
                if (plural != usage.IsPlural)
                {
                    Debug.LogError(plural
                        ? $"Translation `{usage.TranslationId}` is plural, but is used as a singular"
                        : $"Translation `{usage.TranslationId}` is singular, but is used as a plural");
                    continue;
                }

                writer.Write(CreatePoEntry(usage, trForms));
            }
        }

        private static Dictionary<string, string[]> LoadEnglishTranslationMap()
        {
            var json = File.ReadAllText(Application.dataPath + "/../../localization/strdef.json");
            return JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
        }

        private static PoEntry CreatePoEntry(TrUsage usage, string[] trForms)
        {
            var entry = new PoEntry();
            entry.ExtractedComments.Add(TrIdPrefix + usage.TranslationId);
            entry.References.AddRange(usage.Sources);

            if (usage.TranslationId.Contains("{0}") || usage.TranslationId.Contains("{0:"))
                entry.Flags.Add("csharp-format");

            entry.EngStr = trForms[0];
            entry.Translations.Add("");

            if (trForms.Length > 1)
            {
                entry.OptEngStrPlural = trForms[1];
                entry.Translations.Add("");
            }

            return entry;
        }

        [MenuItem("Tools/Localization/Import Non-English Translations from PO-Files", priority = 21)]
        public static void ImportNonEnglishTranslations()
        {
            var path = Application.dataPath + "/../../localization/ru.po";
            foreach (var entry in PoFileReader.Open(path))
            {
                if (entry.EngStr == "")
                    continue;

                if (!TryGetTrId(entry, out var trId))
                {
                    Debug.LogError("Failed to get tr id for string: " + entry.EngStr);
                    continue;
                }

                Debug.Log($"`{trId}` => `{entry.Translations[0]}`");
            }
        }

        private static bool TryGetTrId(PoEntry poEntry, out string trId)
        {
            foreach (var comment in poEntry.ExtractedComments.Where(static comment => comment.StartsWith(TrIdPrefix)))
            {
                trId = comment[TrIdPrefix.Length..];
                return true;
            }

            trId = null;
            return false;
        }
    }
}