using System;

namespace Frog.Core.Save
{
    [Serializable]
    internal class SaveDataInternal
    {
        public string[] AppliedMigrations;
        public string Data;

        public SaveDataInternal(string[] appliedMigrations)
        {
            AppliedMigrations = appliedMigrations;
        }
    }
}