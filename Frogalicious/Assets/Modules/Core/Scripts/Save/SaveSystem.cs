using System;

namespace Frog.Core.Save
{
    public class SaveSystem
    {
        private readonly SaveFileWrapper _saveWrapper;
        private bool _isDirty;

        private FrogSave _save;
        public ref FrogSave Data => ref _save;

        public SaveSystem()
        {
            _saveWrapper = new SaveFileWrapper("save.bin");
            _isDirty = _saveWrapper.Load(Array.Empty<Migration>());
        }

        public void SetDirty() => _isDirty = true;

        public void Save()
        {
            if (!_isDirty)
                return;

            _save.SerialiseTo(ref _saveWrapper.Buffer);
            _saveWrapper.Save();

            _isDirty = false;
        }
    }
}