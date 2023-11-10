using System;
using Frog.Level.Primitives;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class DrawingSettingsView : VisualElement
    {
        private readonly EnumField _layerField = new EnumField("Drawing Layer", DrawingLayer.Tiles);
        private readonly EnumField _tileTypeField = new EnumField("Tile Type", BoardTileType.Nothing);
        private readonly EnumField _objTypeField = new EnumField("Object Type", BoardObjectType.Nothing);

        private DrawingLayer _layer;
        private BoardTileType _tileType;
        private BoardObjectType _objType;

        public DrawingLayer Layer
        {
            get => _layer;
            set => _layerField.value = value;
        }

        public BoardTileType TileType
        {
            get => _tileType;
            set => _tileTypeField.value = value;
        }

        public BoardObjectType ObjectType
        {
            get => _objType;
            set => _objTypeField.value = value;
        }

        public DrawingSettingsView()
        {
            _layerField.RegisterCallback<ChangeEvent<Enum>, DrawingSettingsView>(
                static (e, v) =>
                {
                    v._layer = (DrawingLayer)e.newValue;
                    v.RebuildPanel();
                },
                this
            );

            _tileTypeField.RegisterCallback<ChangeEvent<Enum>, DrawingSettingsView>(
                static (e, v) => v._tileType = (BoardTileType)e.newValue,
                this
            );

            _objTypeField.RegisterCallback<ChangeEvent<Enum>, DrawingSettingsView>(
                static (e, v) => v._objType = (BoardObjectType)e.newValue,
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
                DrawingLayer.Objects => _objTypeField,
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