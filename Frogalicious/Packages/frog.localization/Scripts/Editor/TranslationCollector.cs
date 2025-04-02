using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Frog.Localization.Editor.Gettext;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Frog.Localization.Editor
{
    public static class TranslationCollector
    {
        [MenuItem("Tools/Localization/Generatre Gettext POT File", priority = 20)]
        public static void CollectTranslations()
        {
            var usagesMap = CollectTranslationUsages();
            var translations = LoadTranslations();

            foreach (var (strId, _) in translations)
            {
                if (!usagesMap.ContainsKey(strId))
                    Debug.LogWarning($"Translation `{strId}` is defined but is not used");
            }

            var usagesList = usagesMap.Values.ToList();
            usagesList.Sort((a, b) => string.Compare(a.MsgId, b.MsgId, StringComparison.Ordinal));

            using var writer = new StreamWriter(Application.dataPath + "/../../localization/frog.pot");
            foreach (var usage in usagesList)
            {
                if (!translations.TryGetValue(usage.MsgId, out var msgStrs))
                {
                    Debug.LogError($"Translation `{usage.MsgId}` in not defined");
                    continue;
                }

                if (msgStrs.Length < 1 || msgStrs.Length > 2)
                {
                    Debug.LogError($"Translation `{usage.MsgId}` definition is invalid ({msgStrs.Length} forms)");
                    continue;
                }

                var plural = msgStrs.Length == 2;
                if (plural != usage.IsPlural)
                {
                    Debug.LogError(plural
                        ? $"Translation `{usage.MsgId}` is plural, but is used as a singular"
                        : $"Translation `{usage.MsgId}` is singular, but is used as a plural");
                    continue;
                }

                writer.WriteLine(PoConventions.TrIdCommentPrefix + usage.MsgId);
                foreach (var src in usage.Sources)
                    writer.WriteLine("#: " + src);
                writer.WriteLine("#, csharp-format");

                writer.WriteLine("msgid \"" + Escape(msgStrs[0]) + "\"");
                if (plural)
                    writer.WriteLine("msgid_plural \"" + Escape(msgStrs[1]) + "\"");

                writer.WriteLine("msgstr \"\"");
                writer.WriteLine();
            }
        }

        private static string Escape(string str)
        {
            return str.Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        }

        private static Dictionary<string, TranslationUsage> CollectTranslationUsages()
        {
            var usages = new Dictionary<string, TranslationUsage>();
            PrefabScrapper.Run(usages);
            CodeScrapper.Run(usages);
            return usages;
        }

        private static Dictionary<string, string[]> LoadTranslations()
        {
            var json = File.ReadAllText(Application.dataPath + "/../../localization/strdef.json");
            return JsonConvert.DeserializeObject<Dictionary<string, string[]>>(json);
        }

        [MenuItem("Tools/Localization/Read Gettext PO File", priority = 21)]
        public static void ReadPoFile()
        {
            var path = Application.dataPath + "/../../localization/ru.po";
            foreach (var entry in PoFileReader.Open(path))
            {
                if (entry.EngStr == "")
                    continue;

                Debug.Assert(entry.TranslationId != null);
                Debug.Log($"`{entry.TranslationId}` => `{entry.Translations[0]}`");
            }
        }
    }
}