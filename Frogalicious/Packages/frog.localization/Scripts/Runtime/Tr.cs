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

        public static string Msg(string id)
        {
            return _provider?.GetString(id) ?? id;
        }

        public static string Plu(string id, int count)
        {
            return _provider?.GetPlural(id, count) ?? id;
        }

        public static string MsgIndexed(string id)
        {
            return _provider?.GetString(id) ?? id;
        }

        public static string PluIndexed(string id, int count)
        {
            return _provider?.GetPlural(id, count) ?? id;
        }
    }
}