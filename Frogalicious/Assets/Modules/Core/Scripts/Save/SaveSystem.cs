using System;
using Frog.Collections;

namespace Frog.Core.Save
{
    public class SaveSystem : IDisposable
    {
        private readonly SaveFileWrapper _saveWrapper;
        private bool _isDirty;

        private FrogSave _save;
        public ref FrogSave Data => ref _save;

        public SaveSystem()
        {
            _saveWrapper = SaveFileWrapper.Load("save.bin", Array.Empty<Migration>(), out _isDirty);

            if (_saveWrapper.Buffer.Count() > 0)
            {
                _save.DeserialiseFrom(_saveWrapper.Buffer);
            }
        }

        public void Dispose()
        {
            _saveWrapper.Dispose();
        }

        public void SetDirty() => _isDirty = true;

        public void WriteToDiskIfDirty()
        {
            if (!_isDirty)
                return;

            _save.SerialiseTo(ref _saveWrapper.Buffer);
            _isDirty = !_saveWrapper.WriteToDisk();
        }
    }
}