using System;
using System.Collections.Generic;
using Frog.Level.Primitives;
using UnityEngine;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class DrawingBrushesView : VisualElement
    {
        private const string BrushHolderClass = "brush-holder";
        private const string BrushClass = "brush";
        private const string ActiveBrushClass = "active-brush";

        private readonly Dictionary<int, Button> _buttons = new Dictionary<int, Button>();

        public DrawingLayer Layer { get; private set; }
        public BoardColorGroup? ColorGroup { get; private set; }
        public BoardTileType TileType { get; private set; }
        public BoardObjectType ObjectType { get; private set; }

        public DrawingBrushesView(CellSpritesProvider csp)
        {
            Add(new TextElement { text = "Tiles" });
            CreateButtons<BoardTileType>(csp);

            Add(new TextElement { text = "Objects" });
            CreateButtons<BoardObjectType>(csp);

            GetActiveButton().AddToClassList(ActiveBrushClass);
        }

        private void CreateButtons<TBrush>(CellSpritesProvider csp) where TBrush : Enum
        {
            var holder = new VisualElement();
            holder.AddToClassList(BrushHolderClass);

            foreach (var brush in Enum.GetValues(typeof(TBrush)))
            {

                if (IsColoredBrush(brush))
                {
                    foreach (BoardColorGroup color in Enum.GetValues(typeof(BoardColorGroup)))
                    {
                        CreateButton(holder, csp, brush, color);
                    }
                }
                else
                {
                    CreateButton(holder, csp, brush, null);
                }
            }

            Add(holder);
        }

        private void CreateButton(VisualElement holder, CellSpritesProvider csp, object brush, BoardColorGroup? colorGroup)
        {
            var button = new Button();
            var image = new Image { sprite = GetBrushSprite(csp, brush) };

            if (colorGroup.HasValue)
            {
                image.tintColor = colorGroup.Value switch
                {
                    BoardColorGroup.Blue => Color.cyan,
                    BoardColorGroup.Red => Color.red,
                    _ => Color.black,
                };
            }

            button.AddToClassList(BrushClass);
            button.Add(image);
            holder.Add(button);

            button.clicked += () => SetBrush(brush, colorGroup.GetValueOrDefault());

            _buttons.Add(GetBrushButtonKey(brush, colorGroup.GetValueOrDefault()), button);
        }

        private void SetBrush(object brush, BoardColorGroup colorGroup)
        {
            GetActiveButton().RemoveFromClassList(ActiveBrushClass);

            switch (brush)
            {
                case BoardTileType tileType:
                    (Layer, TileType, ColorGroup) = (DrawingLayer.Tiles, tileType, colorGroup);
                    break;
                case BoardObjectType objectType:
                    (Layer, ObjectType, ColorGroup) = (DrawingLayer.Objects, objectType, colorGroup);
                    break;
                default:
                    Debug.LogError($"Unknown drawing brush: {brush.GetType()}, value: {brush}");
                    break;
            }

            GetActiveButton().AddToClassList(ActiveBrushClass);
        }

        private static bool IsColoredBrush(object brush)
        {
            if (brush is BoardTileType tileType)
            {
                return tileType == BoardTileType.Button || tileType == BoardTileType.Spikes;
            }

            return false;
        }

        private static Sprite GetBrushSprite(CellSpritesProvider csp, object brush)
        {
            switch (brush)
            {
                case BoardTileType tileType: return csp.GetTileSprite(tileType);
                case BoardObjectType objectType: return csp.GetObjectSprite(objectType);
                default:
                    Debug.LogError($"Unknown drawing brush: {brush.GetType()}, value: {brush}");
                    return null;
            }
        }

        private static int GetBrushButtonKey(object brush, BoardColorGroup cg)
        {
            return brush switch
            {
                BoardTileType tileType => (DrawingLayer.Tiles, tileType, cg).GetHashCode(),
                BoardObjectType objectType => (DrawingLayer.Objects, objectType, cg).GetHashCode(),
                _ => throw new IndexOutOfRangeException(),
            };
        }

        private Button GetActiveButton()
        {
            var cg = ColorGroup.GetValueOrDefault();
            var key = Layer switch
            {
                DrawingLayer.Tiles => (DrawingLayer.Tiles, TileType, cg).GetHashCode(),
                DrawingLayer.Objects => (DrawingLayer.Objects, ObjectType, cg).GetHashCode(),
                _ => throw new IndexOutOfRangeException(),
            };

            return _buttons[key];
        }
    }

    internal enum DrawingLayer : uint
    {
        Tiles,
        Objects,
    }
}