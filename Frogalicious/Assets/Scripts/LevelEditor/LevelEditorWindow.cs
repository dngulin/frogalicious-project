using System.Linq;
using Frog.Level.Primitives;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor
{
    public class LevelEditorWindow : EditorWindow
    {
        [SerializeField]
        private VisualTreeAsset _visualTreeAsset;

        [SerializeField]
        private LevelEditorIcons _iconsConfig;

        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            var wnd = GetWindow<LevelEditorWindow>();
            wnd.titleContent = new GUIContent("Level");
        }

        private BoardView _board;

        private EnumFlagsField _brushLayers;
        private EnumField _brushTile;
        private EnumField _brushObject;

        public void CreateGUI()
        {
            _visualTreeAsset.CloneTree(rootVisualElement);
            _board = new BoardView();

            _brushLayers = rootVisualElement.Q<EnumFlagsField>("Layers");
            _brushTile = rootVisualElement.Q<EnumField>("TileType");
            _brushObject = rootVisualElement.Q<EnumField>("ObjectType");

            rootVisualElement.Q<ScrollView>("BoardScroll").Add(_board);

            var tilesMap = _iconsConfig.Tiles.ToDictionary(entry => entry.Type, entry => entry.Sprite);
            var objectsMap = _iconsConfig.Objects.ToDictionary(entry => entry.Type, entry => entry.Sprite);

            _board.RegisterCallback<MouseDownEvent>(e =>
            {
                var pos = e.localMousePosition;
                pos.y = _board.layout.height - pos.y;

                var point = new BoardPoint((int)(pos.x / 64f), (int)(pos.y / 64f));

                Debug.Log($"pos: {e.localMousePosition} -> {pos} -> ({point.X}, {point.Y})");

                var layers = (BoardLayers)_brushLayers.value;

                Sprite tileSprite = null, objSprite = null;

                if ((layers & BoardLayers.Tiles) == BoardLayers.Tiles)
                {
                    var tileType = (BoardTileType)_brushTile.value;
                    tileSprite = tilesMap[tileType];

                    if ((layers & BoardLayers.Objects) == BoardLayers.Objects)
                    {
                        var objectType = (BoardObjectType)_brushObject.value;
                        objSprite = objectsMap[objectType];
                    }
                }

                _board.SetSprites(point, new CellSprites(tileSprite, objSprite));
            });
        }
    }
}
