using System;
using Frog.Level.Primitives;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.Config
{
    [CreateAssetMenu(menuName = "LevelEditor/IconsConfig", fileName = nameof(LevelEditorIcons))]
    public class LevelEditorIcons : ScriptableObject
    {
        public TileSprite[] Tiles;
        public ObjectSprite[] Objects;
    }

    [Serializable]
    public struct TileSprite
    {
        public BoardTileType Type;
        public Sprite Sprite;
    }

    [Serializable]
    public struct ObjectSprite
    {
        public BoardObjectType Type;
        public Sprite Sprite;
    }

    [CustomPropertyDrawer(typeof(TileSprite))]
    public class TileSpritePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

            container.Add(new EnumField
            {
                bindingPath = nameof(TileSprite.Type),
                style = { flexGrow = 1f, width = new StyleLength() },
            });
            container.Add(new ObjectField
            {
                objectType = typeof(Sprite),
                bindingPath = nameof(TileSprite.Sprite),
                style = { flexGrow = 2f, width = new StyleLength() },
            });

            return container;
        }
    }

    [CustomPropertyDrawer(typeof(ObjectSprite))]
    public class ObjectSpritePropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            container.style.flexDirection = new StyleEnum<FlexDirection>(FlexDirection.Row);

            container.Add(new EnumField
            {
                bindingPath = nameof(ObjectSprite.Type),
                style = { flexGrow = 1f, width = new StyleLength() },
            });
            container.Add(new ObjectField
            {
                objectType = typeof(Sprite),
                bindingPath = nameof(ObjectSprite.Sprite),
                style = { flexGrow = 2f, width = new StyleLength() },
            });

            return container;
        }
    }
}