using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Frog.Localization.Editor
{
    public static class TranslationCollector
    {
        [MenuItem("Tools/Localization/Collect String Ids From Project", priority = 20)]
        public static void CollectTranslations()
        {
            var entries = new Dictionary<string, LocalizationEntry>();
            PrefabScrapper.Run(entries);
            CodeScrapper.Run(entries);

            var flatten = entries.Values.ToList();
            flatten.Sort((a, b) => string.Compare(a.MsgId, b.MsgId, StringComparison.Ordinal) );

            foreach (var entry in flatten)
            {
                Debug.Log($"{entry.MsgId} - {entry.Sources.First()}");
            }
        }
    }
}