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
        public BoardObjectHandle Object;
        public BoardTileHandle Tile;
    }

    [Serializable]
    public struct BoardObjectHandle
    {
        public BoardObjectType Type;
        public ushort Variant;
    }

    [Serializable]
    public struct BoardTileHandle
    {
        public BoardTileType Type;
        public ushort Variant;

        public readonly BoardColorGroup Color => (BoardColorGroup)Variant;
        public readonly BoardDirection Direction => (BoardDirection)Variant;
    }
}