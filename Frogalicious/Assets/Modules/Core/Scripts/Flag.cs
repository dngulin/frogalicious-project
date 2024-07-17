using UnityEngine;

namespace Frog.Core
{
    public struct Flag
    {
        private bool _isSet;

        public readonly bool IsSet => _isSet;

        public void Set() => _isSet = true;

        public bool TryReset()
        {
            if (!_isSet)
                return false;

            _isSet = false;
            return true;
        }
    }

    public static class FlagExtensions
    {
        public static void SetAssertive(ref this Flag flag)
        {
            Debug.Assert(!flag.IsSet, "Flag is already set!");
            flag.Set();
        }

        public static void SetIf(ref this Flag flag, bool condition)
        {
            if (condition)
                flag.SetAssertive();
        }
    }
}