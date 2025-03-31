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

        /// <summary>
        /// Get a localized string by a string literal id.
        /// Calls to this method are analyzed to collect used string ids.
        /// If string id is already indexed for usages, use the <see cref="IndexedMsg"/> method instead.
        /// </summary>
        /// <param name="id">string id, should be a string literal</param>
        /// <returns>localized string, or the id itself if a translation provider is not set</returns>
        public static string Msg(string id)
        {
            return _provider?.GetString(id) ?? id;
        }

        /// <summary>
        /// Get a localized plural string by a string literal id.
        /// Calls to this method are analyzed to collect used string ids.
        /// If string id is already indexed for usages, use the <see cref="IndexedPlu"/> method instead.
        /// </summary>
        /// <param name="id">string id, should be a string literal</param>
        /// <param name="count">count, used to select a plural form</param>
        /// <returns>localized string, or the id itself if a translation provider is not set</returns>
        public static string Plu(string id, int count)
        {
            return _provider?.GetPlural(id, count) ?? id;
        }

        /// <summary>
        /// Get a localized string by id.
        /// Use this method if the id is not defined in code (e.g. in a prefab or in a ScriptableObject).
        /// The string id source should provide an indexing method to collect known string ids.
        /// </summary>
        /// <param name="id">string id</param>
        /// <returns>localized string, or the id itself if a translation provider is not set</returns>
        public static string IndexedMsg(string id)
        {
            return _provider?.GetString(id) ?? id;
        }

        /// <summary>
        /// Get a localized plural string by id.
        /// Use this method if the id is not defined in code (e.g. in a prefab or in a ScriptableObject).
        /// The string id source should provide an indexing method to collect known string ids.
        /// </summary>
        /// <param name="id">string id, should be a string literal</param>
        /// <param name="count">count, used to select a plural form</param>
        /// <returns>localized string, or the id itself if a translation provider is not set</returns>
        public static string IndexedPlu(string id, int count)
        {
            return _provider?.GetPlural(id, count) ?? id;
        }
    }
}