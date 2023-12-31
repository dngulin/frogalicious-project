using Frog.Level.Collections;
using Frog.Level.Data;
using Frog.Level.Primitives;

namespace Frog.LevelEditor.View
{
    internal static class BoardGridViewExtensions
    {
        public static void Reset(this BoardGridView boardGridView, LevelData levelData, CellSpritesProvider csp)
        {
            boardGridView.ChangeSize(levelData.Width, levelData.Height);

            for (var y = 0; y < levelData.Height; y++)
            for (var x = 0; x < levelData.Width; x++)
            {
                var point = new BoardPoint(x, y);
                ref readonly var cell = ref levelData.AsBoardGrid().RefAt(point);
                boardGridView.SetSprites(point, csp.GetSprites(cell));
            }
        }
    }
}