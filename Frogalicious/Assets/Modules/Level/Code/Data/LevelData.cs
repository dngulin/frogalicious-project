using System;
using Frog.Level.Primitives;
using UnityEngine;

namespace Frog.Level.Data
{
    [CreateAssetMenu(menuName = "Level Data", fileName = nameof(LevelData))]
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
        public BoardObjectType ObjectType;
    }
}