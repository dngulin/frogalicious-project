using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Frog.Localization.Editor
{
    public static class PrefabScrapper
    {
        private const int PathPrefixLen = 7; // "Assets/".Length

        public static void Run(Dictionary<string, TranslationUsage> entries)
        {
            foreach (var guid in AssetDatabase.FindAssets("t:prefab"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                var localizers = asset.GetComponentsInChildren<StaticTextLocalizer>();

                foreach (var localizer in localizers)
                {
                    var str = localizer.GetComponent<TextMeshProUGUI>().text;

                    var prefabPath = path[PathPrefixLen..];
                    var innerPath = GetObjectPath(localizer.transform, skipRootObjName: true);
                    var src = innerPath == "" ? prefabPath : $"{prefabPath}/{innerPath}";

                    if (entries.TryGetValue(str, out var entry))
                    {
                        entry.Sources.Add(src);
                    }
                    else
                    {
                        entry = new TranslationUsage(str, false);
                        entry.Sources.Add(src);
                        entries.Add(str, entry);
                    }
                }
            }
        }

        private static string GetObjectPath(Transform obj, bool skipRootObjName)
        {
            var names = new List<string> { obj.name };
            while (obj.parent != null)
            {
                obj = obj.parent;
                names.Add(obj.name);
            }

            if (skipRootObjName)
                names.RemoveAt(names.Count - 1);

            names.Reverse();
            return string.Join("/", names);
        }
    }
}