using System.Collections.Generic;
using Frog.Level.Primitives;
using UnityEngine.UIElements;

namespace Frog.LevelEditor.View
{
    internal class BoardGridView : VisualElement
    {
        private readonly List<BoardRowView> _rows = new List<BoardRowView>();

        public BoardGridView()
        {
            AddToClassList("board");
        }

        public void SetSprites(in BoardPoint point, in CellSprites sprites)
        {
            if (point.Y >= 0 && point.Y < _rows.Count)
            {
                _rows[point.Y].SetSprites(point.X, sprites);
            }
        }

        public void ChangeSize(int w, int h)
        {
            ChangeHeight(h);
            ChangeWidth(w);

            style.width = 64 * w;
            style.height = 64 * h;
        }

        private void ChangeHeight(int h)
        {
            while (_rows.Count < h)
            {
                var rowView = new BoardRowView();
                _rows.Add(rowView);
                Add(rowView);
            }

            while (_rows.Count > h)
            {
                var idx = _rows.Count - 1;
                _rows.RemoveAt(idx);
                RemoveAt(idx);
            }
        }

        private void ChangeWidth(int w)
        {
            foreach (var rowView in _rows)
                rowView.ChangeWidth(w);
        }
    }
}