namespace Frog.Localization
{
    public abstract class TranslationProvider
    {
        public abstract string GetString(string key);
        public abstract string GetPlural(string key, int count);
    }
}