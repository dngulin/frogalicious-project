using System;
using Frog.Level.Primitives;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class DrawingSettingsView : VisualElement
    {
        private readonly EnumField _layerField = new EnumField("Drawing Layer", DrawingLayer.Tiles);
        private readonly EnumField _tileTypeField = new EnumField("Tile Type", BoardTileType.Nothing);
        private readonly EnumField _objectTypeField = new EnumField("Object Type", BoardObjectType.Nothing);

        private DrawingLayer _layer;
        private BoardTileType _tileType;
        private BoardObjectType _objType;

        public DrawingLayer Layer
        {
            get => _layer;
            set
            {
                if (_layer == value)
                    return;

                _layerField.value = value;
                RebuildPanel();
            }
        }

        public BoardTileType TileType
        {
            get => _tileType;
            set
            {
                if (_tileType != value)
                    _tileTypeField.value = value;
            }
        }

        public BoardObjectType ObjectType
        {
            get => _objType;
            set
            {
                if (_objType != value)
                    _objectTypeField.value = value;
            }
        }

        public DrawingSettingsView()
        {
            _layerField.RegisterCallback<ChangeEvent<Enum>, DrawingSettingsView>(
                static (e, v) => v.Layer = (DrawingLayer)e.newValue,
                this
            );

            _tileTypeField.RegisterCallback<ChangeEvent<Enum>, DrawingSettingsView>(
                static (e, v) => v.TileType = (BoardTileType)e.newValue,
                this
            );

            _objectTypeField.RegisterCallback<ChangeEvent<Enum>, DrawingSettingsView>(
                static (e, v) => v.ObjectType = (BoardObjectType)e.newValue,
                this
            );

            Add(_layerField);
            Add(_tileTypeField);
        }

        private void RebuildPanel()
        {
            RemoveAt(1);

            var element = _layer switch
            {
                DrawingLayer.Tiles => _tileTypeField,
                DrawingLayer.Objects => _layerField,
                _ => throw new ArgumentOutOfRangeException(),
            };

            Add(element);
        }
    }

    internal enum DrawingLayer
    {
        Tiles,
        Objects,
    }
}