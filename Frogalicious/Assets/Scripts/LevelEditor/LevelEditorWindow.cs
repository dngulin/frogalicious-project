using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        [MenuItem("Window/Level Editor")]
        public static void ShowExample()
        {
            LevelEditorWindow wnd = GetWindow<LevelEditorWindow>();
            wnd.titleContent = new GUIContent("LevelEditorWindow");
        }

        public void CreateGUI()
        {
            var tree = _visualTreeAsset.Instantiate();

            tree.Q<VisualElement>("Board").Add(new BoardView());

            rootVisualElement.Add(tree);
        }
    }
}
