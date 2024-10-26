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

        public SaveFileWrapper(string fileName)
        {
            _path = Path.Combine(Application.persistentDataPath, fileName);
            _save = default;
            _fs = null;
            _bw = null;
        }

        public void Dispose()
        {
            _bw?.Dispose();
            _fs?.Dispose();
        }

        public bool Load(Migration[] migrations)
        {
            if (_fs != null)
                throw new InvalidOperationException("Already loaded");

            if (!File.Exists(_path))
            {
                _fs = new FileStream(_path, FileMode.CreateNew, FileAccess.ReadWrite);
                _bw = new BinaryWriter(_fs, Encoding.ASCII, true);

                WriteDefaultInternalState(ref _save, migrations);
                return true;
            }

            try
            {
                _fs = new FileStream(_path, FileMode.Open, FileAccess.ReadWrite);
                _bw = new BinaryWriter(_fs, Encoding.ASCII, true);

                using (var br = new BinaryReader(_fs, Encoding.ASCII, true))
                {
                    _save.DeserialiseFrom(br);
                }
                return TryExecuteMigrations(ref _save, migrations);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load state! {e.GetType()}: {e.Message}\n{e.StackTrace}");
                WriteDefaultInternalState(ref _save, migrations);
                return true;
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