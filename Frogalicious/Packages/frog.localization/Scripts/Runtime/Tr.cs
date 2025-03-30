using System;

namespace Frog.Localization
{
    public static class Tr
    {
        private static TranslationProvider _provider;
        public static event Action ProviderChanged;

        public static void SetTranslationProvider(TranslationProvider provider)
        {
            _provider = provider;
            ProviderChanged?.Invoke();
        }

        public static string Msg(string key)
        {
            return _provider?.GetString(key) ?? key;
        }

        public static string Plu(string key, int count)
        {
            return _provider?.GetPlural(key, count) ?? key;
        }
    }
}