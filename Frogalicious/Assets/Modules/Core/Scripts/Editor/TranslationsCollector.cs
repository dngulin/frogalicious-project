using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Frog.Core.Localization;
using Frog.Core.Save;
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

        private static readonly string ProjectPath = Directory.GetParent(Application.dataPath)!.FullName;
        private static readonly string RepoRootPath = Directory.GetParent(ProjectPath)!.FullName;

        private static readonly string GettextDirPath = Path.Combine(RepoRootPath, "localization");
        private static readonly string StrDefPath = Path.Combine(GettextDirPath, "strdef.json");
        private static readonly string PotPath = Path.Combine(GettextDirPath, "frog.pot");


        [MenuItem("Tools/Localization/Build English Translation & Export POT-File", priority = 20)]
        public static void BuildEnglishTranslation()
        {
            var usages = new Dictionary<string, TrUsage>();
            TrUsagesByCode.AppendTo(usages);
            TrUsagesByStaticTextLocalizer.AppendTo(usages);
            ValidateHashCollisions(usages);

            var engMap = LoadEnglishTranslationMap();
            ValidateUsages(engMap, usages);

            var usagesList = usages.Values.ToList();
            usagesList.Sort((a, b) => string.Compare(a.TranslationId, b.TranslationId, StringComparison.Ordinal));

            var engTranslations = new FrogTranslations();

            using var poWriter = new PoFileWriter(PotPath);
            foreach (var usage in usagesList)
            {
                if (!TryGetEngForms(engMap, usage, out var pluralForms))
                    continue;

                poWriter.Write(CreatePoEntry(usage, pluralForms));

                for (var i = 0; i < pluralForms.Length; i++)
                    engTranslations.Add(usage.TranslationId, (byte)i, pluralForms[i]);
            }

            var trPath = Path.Combine(ProjectPath, LangMappings.GetLangFileName(GameLanguage.English));
            using var trWriter = new BinaryWriter(File.Create(trPath));
            engTranslations.SerialiseTo(trWriter);
        }

        private static void ValidateHashCollisions(Dictionary<string, TrUsage> usages)
        {
            var hashes = new Dictionary<uint, string>();
            foreach (var (trId, usage) in usages)
            {
                var hash = usage.TranslationId.XxHash32Utf16();
                if (hashes.TryGetValue(hash, out var hashedId))
                {
                    Debug.LogError($"TrIds `{hashedId}` and `{usage.TranslationId}` produce the same hash {hash}");
                    continue;
                }

                hashes.Add(hash, trId);
            }
        }

        private static Dictionary<string, string[]> LoadEnglishTranslationMap()
        {
            var json = File.ReadAllText(StrDefPath);
            return JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
        }

        private static void ValidateUsages(Dictionary<string, string[]> engMap, Dictionary<string, TrUsage> usages)
        {
            foreach (var trId in engMap.Keys)
            {
                if (!usages.ContainsKey(trId))
                    Debug.LogWarning($"Translation `{trId}` is defined but is not used");
            }
        }

        private static bool TryGetEngForms(Dictionary<string, string[]> engMap, in TrUsage usage, out string[] trForms)
        {
            if (!engMap.TryGetValue(usage.TranslationId, out trForms))
            {
                Debug.LogError($"Translation `{usage.TranslationId}` is not defined");
                return false;
            }

            if (trForms.Length < 1 || trForms.Length > 2)
            {
                Debug.LogError(
                    $"Translation `{usage.TranslationId}` definition is invalid ({trForms.Length} forms)");
                return false;
            }

            var plural = trForms.Length == 2;
            if (plural != usage.IsPlural)
            {
                Debug.LogError(plural
                    ? $"Translation `{usage.TranslationId}` is plural, but is used as a singular"
                    : $"Translation `{usage.TranslationId}` is singular, but is used as a plural");
                return false;
            }

            return true;
        }

        private static PoEntry CreatePoEntry(TrUsage usage, string[] pluralForms)
        {
            var entry = new PoEntry();
            entry.ExtractedComments.Add(TrIdPrefix + usage.TranslationId);
            entry.References.AddRange(usage.Sources);

            if (usage.TranslationId.Contains("{0}") || usage.TranslationId.Contains("{0:"))
                entry.Flags.Add("csharp-format");

            entry.EngStr = pluralForms[0];
            entry.Translations.Add("");

            if (pluralForms.Length > 1)
            {
                entry.OptEngStrPlural = pluralForms[1];
                entry.Translations.Add("");
            }

            return entry;
        }

        [MenuItem("Tools/Localization/Import Non-English Translations from PO-Files", priority = 21)]
        public static void ImportNonEnglishTranslations()
        {
            var translations = new FrogTranslations();

            var path = Path.Combine(GettextDirPath, "ru.po");
            foreach (var entry in PoFileReader.Open(path))
            {
                if (entry.EngStr == "")
                    continue;

                if (!TryGetTrId(entry, out var trId))
                {
                    Debug.LogError("Failed to get tr id for string: " + entry.EngStr);
                    continue;
                }

                for (var i = 0; i < entry.Translations.Count; i++)
                    translations.Add(trId, (byte)i, entry.Translations[i]);
            }

            var trPath = Path.Combine(ProjectPath, LangMappings.GetLangFileName(GameLanguage.Russian));
            using var trWriter = new BinaryWriter(File.Create(trPath));

            translations.SerialiseTo(trWriter);
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