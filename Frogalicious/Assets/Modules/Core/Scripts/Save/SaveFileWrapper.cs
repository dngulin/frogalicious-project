using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Frog.Collections;
using UnityEngine;

namespace Frog.Core.Save
{
    internal class SaveFileWrapper : IDisposable
    {
        private readonly string _path;
        private SaveInternal _save;

        private FileStream _fs;
        private BinaryWriter _bw;

        public ref RefList<byte> Buffer => ref _save.Data;

        private SaveFileWrapper(string fileName)
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
        }

        public void Dispose()
        {
            _bw?.Dispose();
            _fs?.Dispose();
        }

        public static SaveFileWrapper Load(string fileName, Migration[] migrations, out bool needsSave)
        {
            var wrapper = new SaveFileWrapper(fileName);
            wrapper.LoadInternal(migrations, out needsSave);
            return wrapper;
        }

        private void LoadInternal(Migration[] migrations, out bool needsSave)
        {
            if (!File.Exists(_path))
            {
                _fs = new FileStream(_path, FileMode.CreateNew, FileAccess.ReadWrite);
                _bw = new BinaryWriter(_fs, Encoding.ASCII, true);

                SetupDefaultSave(ref _save, migrations);
                needsSave = true;
                return;
            }

            try
            {
                _fs = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
                _bw = new BinaryWriter(_fs, Encoding.ASCII, true);

                using (var br = new BinaryReader(_fs, Encoding.ASCII, true))
                {
                    _save.DeserialiseFrom(br);
                }

                needsSave = TryExecuteMigrations(ref _save, migrations);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game save! {e.GetType()}: {e.Message}\n{e.StackTrace}");
                SetupDefaultSave(ref _save, migrations);
                needsSave = true;
            }
        }

        public void Save()
        {
            _save.SerialiseTo(_bw);
            _bw.Flush();
            _fs.Flush();
        }

        private static void WriteDefaultInternalState(ref SaveInternal save, Migration[] migrations)
        {
            save.Clear();

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (var migration in migrations)
            {
                ref var migrationInfo = ref save.Migrations.RefAdd();
                migrationInfo.Name.SetFromAsciiString(migration.Name);
                migrationInfo.TimeStamp = timestamp;
            }
        }

        private static bool TryExecuteMigrations(ref SaveInternal save, Migration[] migrations)
        {
            if (save.Migrations.Count() == migrations.Length)
                return false;

            var appliedMigrations = new HashSet<string>(save.Migrations.Count());

            for (var i = 0; i < save.Migrations.Count(); i++)
            {
                ref readonly var migrationInfo = ref save.Migrations.RefReadonlyAt(i);
                appliedMigrations.Add(migrationInfo.Name.ToStringAscii());
            }

            foreach (var migration in migrations)
            {
                if (appliedMigrations.Contains(migration.Name))
                    continue;

                migration.Execute(ref save.Data);

                ref var migrationInfo = ref save.Migrations.RefAdd();
                migrationInfo.Name.SetFromAsciiString(migration.Name);
                migrationInfo.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }

            return true;
        }
    }
}