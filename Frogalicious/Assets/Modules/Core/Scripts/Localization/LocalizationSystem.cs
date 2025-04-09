using System;
using System.IO;
using Frog.Core.Save;
using Frog.Localization;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Frog.Core.Localization
{
    public class LocalizationSystem : IDisposable
    {
        public GameLanguage? Language { get; private set; }

        public async Awaitable LoadLanguage(LanguageSetting setting)
        {
            var language = LangMappings.GetLangForSetting(setting);

            var fileName = LangMappings.GetLangFileName(language);
            var pluralFormula = LangMappings.GetPluralFormula(language);

            var handle = Addressables.LoadAssetAsync<TextAsset>(fileName);
            var data = (await handle.Task).bytes;
            handle.Release();

            var translations = new FrogTranslations();
            translations.DeserialiseFrom(new BinaryReader(new MemoryStream(data)));

            Tr.SetTranslationProvider(new FrogTranslationProvider(translations.ToDictionary(), pluralFormula));
            Language = language;
        }

        public void Dispose()
        {
            Tr.SetTranslationProvider(null);
        }
    }
}