using System;
using Frog.Level.Primitives;
using UnityEngine;

namespace Frog.Level.Data.Migrations.V1
{
    public class LevelData : ScriptableObject
    {
        public ushort Width;
        public ushort Height;
        public CellData[] Cells;
    }

    [Serializable]
    public struct CellData
    {
        public BoardTileType TileType;
        public BoardColorGroup TileColor;
        public BoardDirection TileDirection;

        public BoardObjectType ObjectType;
    }
}