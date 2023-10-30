using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset = default;

        [MenuItem("Window/UI Toolkit/LevelEditorWindow")]
        public static void ShowExample()
        {
            LevelEditorWindow wnd = GetWindow<LevelEditorWindow>();
            wnd.titleContent = new GUIContent("LevelEditorWindow");
        }

        public void CreateGUI()
        {
            rootVisualElement.Add(_visualTreeAsset.Instantiate());
        }
    }
}
