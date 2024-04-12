using System.Linq;
using Frog.Level.Data;
using UnityEditor;
using UnityEngine;

namespace Frog.Level.DataMigrations
{
    public static class LevelDataMigrations
    {
        [MenuItem("Tools/Update LevelData Format")]
        public static void Execute()
        {
            foreach (var path in GetGuids(nameof(LevelData_V1)))
            {
                var (v1, v2) = PrepareInstances<LevelData_V1, LevelData>(path);
                v1.MigrateTo(v2);
                AssetDatabase.CreateAsset(v2, path);
            }

            AssetDatabase.SaveAssets();
        }

        private static string[] GetGuids(string type) =>
            AssetDatabase
                .FindAssets($"t:{type}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

        private static (T1, T2) PrepareInstances<T1, T2>(string path)
            where T1 : ScriptableObject where T2 : ScriptableObject
        {
            return (AssetDatabase.LoadAssetAtPath<T1>(path), ScriptableObject.CreateInstance<T2>());
        }
    }
}