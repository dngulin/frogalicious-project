using Frog.Level.Data;

namespace Frog.LevelEditor.Data
{
    public static class EditorLevelDataConverter
    {
        public static EditorLevelData CreateFrom(LevelData data)
        {
            var editorData = new EditorLevelData();
            editorData.ChangeBoardSize(data.Width, data.Height);

            for (var row = 0; row < data.Height; row++)
            for (var col = 0; col < data.Width; col++)
            {
                ref readonly var cellData = ref data.Cells[row * data.Width + col];

                editorData.Rows[row][col] = new EditorCellData
                {
                    TileType = cellData.TileType,
                    ObjectType = cellData.ObjectType,
                };
            }

            return editorData;
        }

        public static void WriteTo(this EditorLevelData editorData, LevelData data)
        {
            data.Height = (ushort) editorData.Rows.Count;
            data.Width = (ushort) (data.Height > 0 ? editorData.Rows[0].Count : 0);
            data.Cells = new BoardCellConfig[data.Width * data.Height];

            for (var row = 0; row < data.Height; row++)
            for (var col = 0; col < data.Width; col++)
            {
                var editorCellData = editorData.Rows[row][col];
                ref var cellData = ref data.Cells[row * data.Width + col];

                cellData.TileType = editorCellData.TileType;
                cellData.ObjectType = editorCellData.ObjectType;
            }
        }
    }
}