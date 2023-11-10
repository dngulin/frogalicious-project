using System.Collections.Generic;

namespace Frog.LevelEditor.Data
{
    internal static class EditorLevelDataExtensions
    {
        public static bool ChangeBoardSize(this EditorLevelData level, int w, int h)
        {
            var hChanged = ChangeBoardHeight(level, h);
            var wChanged = ChangeBoardWidth(level, w);
            return hChanged || wChanged;
        }

        private static bool ChangeBoardHeight(EditorLevelData level, int h)
        {
            if (level.Rows.Count < h)
            {
                while (level.Rows.Count < h) level.Rows.Add(new List<EditorCellData>());
                return true;
            }

            if (level.Rows.Count > h)
            {
                while (level.Rows.Count > h) level.Rows.RemoveAt(level.Rows.Count - 1);
                return true;
            }

            return false;
        }

        private static bool ChangeBoardWidth(EditorLevelData level, int w)
        {
            var updated = false;

            foreach (var row in level.Rows)
            {
                if (row.Count < w)
                {
                    while (row.Count < w) row.Add(default);
                    updated = true;
                }
                else if (row.Count > w)
                {
                    while (row.Count > w) row.RemoveAt(row.Count - 1);
                    updated = true;
                }
            }

            return updated;
        }
    }
}