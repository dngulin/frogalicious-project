using System.Collections.Generic;
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

        private Dictionary<BoardTileType, Sprite> _tileIcons;
        private Dictionary<BoardObjectType, Sprite> _objectIcons;

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

            _tileIcons = _iconsConfig.Tiles.ToDictionary(entry => entry.Type, entry => entry.Sprite);
            _objectIcons = _iconsConfig.Objects.ToDictionary(entry => entry.Type, entry => entry.Sprite);

            _board.RegisterCallback<MouseDownEvent>(e => HandleMouseEvent(e.localMousePosition));
            _board.RegisterCallback<MouseMoveEvent>(e =>
            {
                if ((e.pressedButtons & 1) == 1)
                {
                    HandleMouseEvent(e.localMousePosition);
                }
            });
        }

        private void HandleMouseEvent(in Vector2 eventLocalPosition)
        {
            var pos = eventLocalPosition;
            pos.y = _board.layout.height - pos.y;

            var point = new BoardPoint((int)(pos.x / 64f), (int)(pos.y / 64f));

            var layers = (BoardLayers)_brushLayers.value;

            Sprite tileSprite = null, objSprite = null;

            if ((layers & BoardLayers.Tiles) == BoardLayers.Tiles)
            {
                var tileType = (BoardTileType)_brushTile.value;
                tileSprite = _tileIcons[tileType];

                if ((layers & BoardLayers.Objects) == BoardLayers.Objects)
                {
                    var objectType = (BoardObjectType)_brushObject.value;
                    objSprite = _objectIcons[objectType];
                }
            }

            _board.SetSprites(point, new CellSprites(tileSprite, objSprite));
        }
    }
}
