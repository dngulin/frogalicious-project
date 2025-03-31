namespace Frog.Localization
{
    public abstract class TranslationProvider
    {
        public abstract string GetString(string id);
        public abstract string GetPlural(string id, int count);
    }
}