using System.Collections.Generic;
using Frog.Level.Primitives;

namespace Frog.LevelEditor.Data
{
    public class EditorLevelData
    {
        public List<List<EditorCellData>> Rows = new List<List<EditorCellData>>();
    }

    public struct EditorCellData
    {
        public BoardTileType TileType;
        public BoardObjectType ObjectType;
    }
}