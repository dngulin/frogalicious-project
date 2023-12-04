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
                var button = new Button();
                var image = new Image { sprite = GetBrushSprite(csp, brush) };

                button.AddToClassList(BrushClass);
                button.Add(image);
                holder.Add(button);

                button.clicked += () => SetBrush(brush);

                _buttons.Add(GetBrushButtonKey(brush), button);
            }

            Add(holder);
        }

        private void SetBrush(object brush)
        {
            GetActiveButton().RemoveFromClassList(ActiveBrushClass);

            switch (brush)
            {
                case BoardTileType tileType:
                    (Layer, TileType) = (DrawingLayer.Tiles, tileType);
                    break;
                case BoardObjectType objectType:
                    (Layer, ObjectType) = (DrawingLayer.Objects, objectType);
                    break;
                default:
                    Debug.LogError($"Unknown drawing brush: {brush.GetType()}, value: {brush}");
                    break;
            }

            GetActiveButton().AddToClassList(ActiveBrushClass);
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

        private static int GetBrushButtonKey(object brush)
        {
            return brush switch
            {
                BoardTileType tileType => (DrawingLayer.Tiles, tileType).GetHashCode(),
                BoardObjectType objectType => (DrawingLayer.Objects, objectType).GetHashCode(),
                _ => throw new IndexOutOfRangeException(),
            };
        }

        private Button GetActiveButton()
        {
            var key = Layer switch
            {
                DrawingLayer.Tiles => (DrawingLayer.Tiles, TileType).GetHashCode(),
                DrawingLayer.Objects => (DrawingLayer.Objects, ObjectType).GetHashCode(),
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