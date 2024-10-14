using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Frog.Core.Save
{
    public class SaveSystem<TData> where TData : class, new()
    {
        private readonly string _path;
        private readonly SaveDataInternal _save;

        private readonly TData _data;
        public TData Data => _data;

        private bool _isDirty;

        public SaveSystem(Migration[] migrations)
        {
            _path = Path.Combine(Application.persistentDataPath, "save.json");

            if (!File.Exists(_path))
            {
                _save = new SaveDataInternal(migrations.Select(static m => m.Name).ToArray());
                _data = new TData();
                _isDirty = true;
            }
            else
            {
                _save = JsonUtility.FromJson<SaveDataInternal>(File.ReadAllText(_path));
                _isDirty = TryExecuteMigrations(migrations);
                _data = JsonUtility.FromJson<TData>(_save.Data);
                _save.Data = null;
            }
        }

        public void SetDirty() => _isDirty = true;

        public void Save()
        {
            if (!_isDirty)
                return;

            _save.Data = JsonUtility.ToJson(_data);
            File.WriteAllText(_path, JsonUtility.ToJson(_save));
            _save.Data = null;

            _isDirty = false;
        }

        private bool TryExecuteMigrations(Migration[] migrations)
        {
            var appliedMigrations = new HashSet<string>(_save.AppliedMigrations);

            foreach (var migration in migrations)
            {
                if (appliedMigrations.Contains(migration.Name))
                    continue;

                _save.Data = migration.Execute(_save.Data);
                appliedMigrations.Add(migration.Name);
            }

            if (appliedMigrations.Count == _save.AppliedMigrations.Length)
                return false;

            _save.AppliedMigrations = appliedMigrations.ToArray();
            return true;
        }
    }
}