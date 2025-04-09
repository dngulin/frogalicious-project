using System;
using Frog.Core.Save;
using Frog.Localization;
using UnityEngine;

namespace Frog.Core.Localization
{
    public static class LangMappings
    {
        public static GameLanguage GetLangForSetting(LanguageSetting setting)
        {
            return setting switch
            {
                LanguageSetting.Detect => DetectLanguage(),
                LanguageSetting.English => GameLanguage.English,
                LanguageSetting.Russian => GameLanguage.Russian,
                _ => throw new IndexOutOfRangeException(),
            };
        }

        private static GameLanguage DetectLanguage()
        {
            return Application.systemLanguage switch
            {
                SystemLanguage.English => GameLanguage.English,
                SystemLanguage.Russian => GameLanguage.Russian,
                _ => GameLanguage.English,
            };
        }

        public static Func<int, int> GetPluralFormula(GameLanguage language)
        {
            return language switch
            {
                GameLanguage.English => static n => PluralFormula.English(n),
                GameLanguage.Russian => static n => PluralFormula.Russian(n),
                _ => throw new IndexOutOfRangeException(),
            };
        }

        public static string GetLangFileName(GameLanguage language)
        {
            return language switch
            {
                GameLanguage.English => "Assets/Translations/en.bytes",
                GameLanguage.Russian => "Assets/Translations/ru.bytes",
                _ => throw new IndexOutOfRangeException(),
            };
        }
    }
}