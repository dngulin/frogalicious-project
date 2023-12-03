using System;
using Frog.Level.Primitives;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class DrawingSettingsView : VisualElement
    {
        public DrawingLayer Layer { get; private set; }
        public BoardTileType TileType { get; private set; }
        public BoardObjectType ObjectType { get; private set; }

        public DrawingSettingsView(CellSpritesProvider csp)
        {
            Add(new TextElement { text = "Tiles" });
            CreateButtons<BoardTileType>(csp);

            Add(new TextElement { text = "Objects" });
            CreateButtons<BoardObjectType>(csp);
        }

        private void CreateButtons<TBrush>(CellSpritesProvider csp) where TBrush : Enum
        {
            var holder = CreateButtonsPanel();

            foreach (var brush in Enum.GetValues(typeof(TBrush)))
            {
                var button = new Button { style = { width = 32, height = 32 } };
                var image = new Image { sprite = GetBrushSprite(csp, brush) };
                button.Add(image);
                holder.Add(button);

                button.clicked += () => SetBrush(brush);
            }

            Add(holder);
        }

        private void SetBrush(object brush)
        {
            switch (brush)
            {
                case BoardTileType tileType:
                    Layer = DrawingLayer.Tiles;
                    TileType = tileType;
                    break;

                case BoardObjectType objectType:
                    Layer = DrawingLayer.Objects;
                    ObjectType = objectType;
                    break;

                default:
                    Debug.LogError($"Unknown drawing brush: {brush.GetType()}, value: {brush}");
                    break;
            }
        }

        private static Sprite GetBrushSprite(CellSpritesProvider csp, object brush)
        {
            switch (brush)
            {
                case BoardTileType tileType:
                    return csp.GetTileSprite(tileType);

                case BoardObjectType objectType:
                    return csp.GetObjectSprite(objectType);

                default:
                    Debug.LogError($"Unknown drawing brush: {brush.GetType()}, value: {brush}");
                    return null;
            }
        }

        private static VisualElement CreateButtonsPanel()
        {
            return new VisualElement
            {
                style = { flexDirection = FlexDirection.Row, flexWrap = Wrap.Wrap },
            };
        }
    }

    internal enum DrawingLayer
    {
        Tiles,
        Objects,
    }
}