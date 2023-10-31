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
            wnd.titleContent = new GUIContent("Level");
        }

        public void CreateGUI()
        {
            _visualTreeAsset.CloneTree(rootVisualElement);

            rootVisualElement
                .Q<ScrollView>("BoardScroll")
                .Add(new BoardView());
        }
    }
}
