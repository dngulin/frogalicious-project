using System;
using Frog.Level.Primitives;
using UnityEngine;

namespace Frog.Level.Data.Migrations
{
    public class LevelData_V1 : ScriptableObject
    {
        public ushort Width;
        public ushort Height;
        public CellData_V1[] Cells;
    }

    [Serializable]
    public struct CellData_V1
    {
        public BoardTileType TileType;
        public BoardColorGroup TileColor;
        public BoardDirection TileDirection;

        public BoardObjectType ObjectType;
    }

    public static class LevelData_V1_Migration
    {
        public static void MigrateTo(this LevelData_V1 v1, LevelData v2)
        {
            v2.Width = v1.Width;
            v2.Height = v1.Height;

            v2.Cells = new CellData[v1.Cells.Length];
            for (int i = 0; i < v2.Cells.Length; i++)
                v1.Cells[i].MigrateTo(ref v2.Cells[i]);
        }

        public static void MigrateTo(this in CellData_V1 v1, ref CellData v2)
        {
            v2.Object.Type = v1.ObjectType;

            v2.Tile.Type = v1.TileType;
            v2.Tile.SetVariant(v1.TileColor, v1.TileDirection);
        }
    }
}