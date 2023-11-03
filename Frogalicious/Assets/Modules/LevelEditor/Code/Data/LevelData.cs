using System.Collections.Generic;
using Frog.Level.Primitives;

namespace Frog.LevelEditor.Data
{
    public class LevelData
    {
        public List<List<CellData>> Rows = new List<List<CellData>>();
    }

    public struct CellData
    {
        public BoardTileType TileType;
        public BoardObjectType ObjectType;
    }
}