using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class BoardRowView : VisualElement
    {
        private readonly List<BoardCellView> _cells = new List<BoardCellView>();

        public BoardRowView()
        {
            AddToClassList("board-row");

            for (var i = 0; i < 9; i++)
            {
                var cell = new BoardCellView();
                _cells.Add(cell);
                Add(cell);
            }
        }

        public void SetSprites(int idx, in CellSprites sprites)
        {
            if (idx >= 0 && idx < _cells.Count)
            {
                _cells[idx].SetSprites(sprites);
            }
        }

        public void ChangeWidth(int w)
        {
            while (_cells.Count < w)
            {
                var cellView = new BoardCellView();
                _cells.Add(cellView);
                Add(cellView);
            }

            while (_cells.Count > w)
            {
                var idx = _cells.Count - 1;
                _cells.RemoveAt(idx);
                RemoveAt(idx);
            }
        }
    }
}