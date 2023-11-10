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
        public BoardCellConfig[] Cells;
    }

    [Serializable]
    public struct BoardCellConfig
    {
        public BoardTileType Tile;
        public BoardObjectType Object;
    }
}