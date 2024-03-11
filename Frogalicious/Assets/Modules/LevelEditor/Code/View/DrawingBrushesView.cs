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

        private DrawingBrushState _brush;

        public ref readonly DrawingBrushState Brush => ref _brush;

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
                        CreateButton(holder, csp, brush, color, null);
                }
                else if (IsDirectedBrush(brush))
                {
                    foreach (BoardDirection direction in Enum.GetValues(typeof(BoardDirection)))
                        CreateButton(holder, csp, brush, null, direction);
                }
                else
                {
                    CreateButton(holder, csp, brush, null, null);
                }
            }

            Add(holder);
        }

        private void CreateButton(VisualElement holder, CellSpritesProvider csp, object brush, BoardColorGroup? optColor, BoardDirection? optDirection)
        {
            var button = new Button();
            var image = new Image { sprite = GetBrushSprite(csp, brush) };

            if (optColor.HasValue)
                image.tintColor = CellSpritesProvider.GetTintColor(optColor.Value);

            if (optDirection.HasValue)
                image.transform.rotation = CellSpritesProvider.GetRotation(optDirection.Value);

            button.AddToClassList(BrushClass);
            button.Add(image);
            holder.Add(button);

            var color = optColor.GetValueOrDefault();
            var direction = optDirection.GetValueOrDefault();

            button.clicked += () => SetBrush(brush, color, direction);

            _buttons.Add(GetBrushButtonKey(brush, color, direction), button);
        }

        private void SetBrush(object brush, BoardColorGroup color, BoardDirection direction)
        {
            GetActiveButton().RemoveFromClassList(ActiveBrushClass);

            switch (brush)
            {
                case BoardTileType tileType:
                    _brush.Update(tileType, color, direction);
                    break;
                case BoardObjectType objectType:
                    Debug.Assert(color == default);
                    _brush.Update(objectType);
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

        private static bool IsDirectedBrush(object brush)
        {
            if (brush is BoardTileType tileType)
            {
                return tileType == BoardTileType.Spring;
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

        private static int GetBrushButtonKey(object brush, BoardColorGroup color, BoardDirection direction)
        {
            var brushState = new DrawingBrushState {Color = color};

            switch (brush)
            {
                case BoardTileType tileType:
                    brushState.Layer = DrawingLayer.Tiles;
                    brushState.TileType = tileType;
                    brushState.Direction = direction;
                    break;
                case BoardObjectType objType:
                    brushState.Layer = DrawingLayer.Objects;
                    brushState.ObjectType = objType;
                    break;
                default:
                    throw new IndexOutOfRangeException();
            }

            return brushState.CalculateHash();
        }

        private Button GetActiveButton()
        {
            return _buttons[_brush.CalculateHash()];
        }
    }
}