using UnityEngine;

namespace Frog.Core
{
    public struct Flag
    {
        private bool _isSet;

        public bool IsSet => _isSet;

        public void Set() => _isSet = true;

        public bool Reset()
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
    }
}