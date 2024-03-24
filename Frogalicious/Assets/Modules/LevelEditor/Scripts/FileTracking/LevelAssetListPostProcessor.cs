using System.Collections.Generic;
using UnityEditor;

namespace Frog.LevelEditor.FileTracking
{
    public class LevelAssetListPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!EditorWindow.HasOpenInstances<LevelEditorWindow>())
                return;

            var window = EditorWindow.GetWindow<LevelEditorWindow>(title: null, focus: false);

            // Check if GUI is already created
            if (window.rootVisualElement.childCount == 0)
                return;

            var renames = new Dictionary<string, string>(movedAssets.Length);
            for (var i = 0; i < movedFromAssetPaths.Length; i++)
            {
                renames.Add(movedFromAssetPaths[i], movedAssets[i]);
            }

            window.RebuildLevelList(renames);
        }
    }
}