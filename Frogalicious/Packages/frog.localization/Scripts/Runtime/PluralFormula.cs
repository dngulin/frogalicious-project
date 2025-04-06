using UnityEngine;

namespace Frog.Localization
{
    public static class PluralFormula
    {
        public static int NoPlurals(int number) => 0;

        public static int NonOneIsPlural(int number) => Mathf.Abs(number) == 1 ? 0 : 1;
        public static int MoreThanOneIsPlural(int number) => Mathf.Abs(number) > 1 ? 0 : 1;

        /// <summary>
        /// Three plural forms:
        /// <list type="bullet">
        /// <item>Numbers ending in 1 (except 11)</item>
        /// <item>Numbers ending in 2, 3, 4 (except 12, 13, 14)</item>
        /// <item>Numbers ending in 5, 6, 7, 8, 9, 0 and 11, 12, 13, 14 </item>
        /// </list>
        /// Applicable to Belarusian, Bosnian, Croatian, Montenegro, Russian, Serbian and Ukrainian languages.
        /// </summary>
        /// <param name="number">number to classify the plural form</param>
        /// <returns>plural form index</returns>
        public static int SouthAndEastSlavic(int number)
        {
            var abs = Mathf.Abs(number);
            return (abs % 10) switch
            {
                1 when abs % 100 != 11 => 0,
                2 when abs % 100 != 12 => 1,
                3 when abs % 100 != 13 => 1,
                4 when abs % 100 != 14 => 1,
                _ => 2,
            };
        }
    }
}