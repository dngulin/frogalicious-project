using Frog.Level.Collections;
using Frog.Level.Data;
using Frog.Level.Primitives;
using UnityEditor;
using UnityEngine;

namespace Frog.LevelEditor.Data
{
    public static class LevelDataEditorExtensions
    {
        public static void SaveAsset(this LevelData levelData)
        {
            EditorUtility.SetDirty(levelData);
            AssetDatabase.SaveAssetIfDirty(levelData);
        }

        public static bool ChangeBoardSize(this LevelData levelData, ushort w, ushort h)
        {
            if (levelData.Cells == null || levelData.Cells.Length != levelData.Width * levelData.Height)
            {
                levelData.Width = w;
                levelData.Height = h;
                levelData.Cells = new CellData[w * h];
                return true;
            }

            if (levelData.Width == w && levelData.Height == h)
                return false;

            var newGrid = new BoardGrid<CellData>(new CellData[w * h], w, h);
            var oldGrid = levelData.AsBoardGrid();

            for (var y = 0; y < Mathf.Min(h, levelData.Height); y++)
            for (var x = 0; x < Mathf.Min(w, levelData.Width); x++)
            {
                var point = new BoardPoint(x, y);
                newGrid.RefAtMut(point) = oldGrid.RefAt(point);
            }

            levelData.Width = newGrid.Width;
            levelData.Height = newGrid.Height;
            levelData.Cells = newGrid.Cells;
            return true;

        }
    }
}