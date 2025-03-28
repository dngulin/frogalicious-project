using System;
using Frog.Collections;

namespace Frog.Core.Save
{
    public class SaveSystem : IDisposable
    {
        private readonly SaveFileWrapper _save;
        private bool _isDirty;

        public ref FrogSave Data => ref _save.Data;

        public SaveSystem()
        {
            _save = SaveFileWrapper.Load("save.bin", Array.Empty<Migration>(), out _isDirty);
        }

        public void Dispose()
        {
            _save.Dispose();
        }

        public void SetDirty() => _isDirty = true;

        public void WriteToDiskIfDirty()
        {
            if (!_isDirty)
                return;

            _isDirty = !_save.WriteToDisk();
        }
    }
}