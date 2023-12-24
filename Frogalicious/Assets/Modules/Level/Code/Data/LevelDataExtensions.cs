using Frog.Level.Collections;

namespace Frog.Level.Data
{
    public static class LevelDataExtensions {
        public static BoardGrid<CellData> AsBoardGrid(this LevelData data)
        {
            return new BoardGrid<CellData>(data.Cells, data.Width, data.Height);
        }
    }
}