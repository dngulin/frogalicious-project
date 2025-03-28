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
        private readonly FileStream _fs;
        private readonly BinaryWriter _bw;

        private SaveHeader _header;
        private FrogSave _data;

        public ref FrogSave Data => ref _data;

        private SaveFileWrapper(FileStream fs)
        {
            _fs = fs;
            _bw = new BinaryWriter(fs, Encoding.ASCII, true);
        }

        public void Dispose()
        {
            _bw?.Dispose();
            _fs?.Dispose();
        }

        public static SaveFileWrapper Load(string fileName, Migration[] migrations, out bool needsSave)
        {
            var path = Path.Combine(Application.persistentDataPath, fileName);
            if (!File.Exists(path))
            {
                var fs = new FileStream(path, FileMode.CreateNew, FileAccess.ReadWrite);
                var wrapper = new SaveFileWrapper(fs);

                SetupDefaultHeader(ref wrapper._header, migrations);
                needsSave = true;
                return wrapper;
            }
            else
            {
                var fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
                var wrapper = new SaveFileWrapper(fs);
                needsSave = wrapper.LoadFormFile(migrations);
                return wrapper;
            }
        }

        private bool LoadFormFile(Migration[] migrations)
        {
            try
            {
                using (var fileReader = new BinaryReader(_fs, Encoding.ASCII, true))
                {
                    _header.DeserialiseFrom(fileReader);
                    if (_header.Migrations.Count() == migrations.Length)
                    {
                        _data.DeserialiseFrom(fileReader);
                        return false;
                    }
                }

                using (var ms = new MemoryStream())
                {
                    _fs.CopyTo(ms);
                    ExecuteMigrations(ref _header, migrations, ms);

                    using (var memoryReader = new BinaryReader(ms, Encoding.ASCII, true))
                    {
                        _data.DeserialiseFrom(memoryReader);
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load game save! {e.GetType()}: {e.Message}\n{e.StackTrace}");
                SetupDefaultHeader(ref _header, migrations);
                return true;
            }
        }

        public bool WriteToDisk()
        {
            try
            {
                _fs.Position = 0;
                _header.SerialiseTo(_bw);
                _data.SerialiseTo(_bw);
                _bw.Flush();
                _fs.Flush();
                _fs.SetLength(_fs.Position);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save game state! {e.GetType()}: {e.Message}\n{e.StackTrace}");
                return false;
            }
        }

        private static void SetupDefaultHeader(ref SaveHeader header, Migration[] migrations)
        {
            header.Clear();

            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            foreach (var migration in migrations)
            {
                ref var migrationInfo = ref header.Migrations.RefAdd();
                migrationInfo.Name.AppendAsciiString(migration.Name);
                migrationInfo.TimeStamp = timestamp;
            }
        }

        private static void ExecuteMigrations(ref SaveHeader save, Migration[] migrations, MemoryStream ms)
        {
            var appliedMigrations = new HashSet<string>(save.Migrations.Count());
            foreach (ref readonly var mInfo in save.Migrations.RefReadonlyIter())
            {
                appliedMigrations.Add(mInfo.Name.ToStringAscii());
            }

            foreach (var migration in migrations)
            {
                if (appliedMigrations.Contains(migration.Name))
                    continue;

                migration.Execute(ms);

                ref var migrationInfo = ref save.Migrations.RefAdd();
                migrationInfo.Name.AppendAsciiString(migration.Name);
                migrationInfo.TimeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
    }
}