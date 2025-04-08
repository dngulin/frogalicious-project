using System.Runtime.CompilerServices;
using UnityEngine;

namespace Frog.Localization
{

    /// <summary>
    /// Based on https://docs.translatehouse.org/projects/localization-guide/en/latest/l10n/pluralforms.html
    /// </summary>
    public static class PluralFormula
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NoPlurals(int number) => 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NonOneIsPlural(int number) => Mathf.Abs(number) == 1 ? 0 : 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        public static int Afrikaans(int number) => NonOneIsPlural(number);

        public static int Arabic(int number)
        {
            var abs = Mathf.Abs(number);
            return abs switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                _ => (abs % 100) switch
                {
                    >= 3 and <= 10 => 3,
                    >= 11 => 4,
                    _ => 5,
                },
            };
        }

        public static int Basque(int number) => NonOneIsPlural(number);
        public static int Belarusian(int number) => SouthAndEastSlavic(number);
        public static int Bulgarian(int number) => NonOneIsPlural(number);
        public static int Catalan(int number) => NonOneIsPlural(number);
        public static int Chinese(int number) => NoPlurals(number);

        public static int Czech(int number)
        {
            var abs = Mathf.Abs(number);
            return abs switch
            {
                1 => 0,
                2 => 1,
                3 => 1,
                4 => 1,
                _ => 2,
            };
        }

        public static int Danish(int number) => NonOneIsPlural(number);
        public static int Dutch(int number) => NonOneIsPlural(number);
        public static int English(int number) => NonOneIsPlural(number);
        public static int Estonian(int number) => NonOneIsPlural(number);
        public static int Faroese(int number) => NonOneIsPlural(number);
        public static int Finnish(int number) => NonOneIsPlural(number);
        public static int French(int number) => MoreThanOneIsPlural(number);
        public static int German(int number) => NonOneIsPlural(number);
        public static int Greek(int number) => NonOneIsPlural(number);
        public static int Hungarian(int number) => NonOneIsPlural(number);

        public static int Icelandic(int number)
        {
            var abs = Mathf.Abs(number);
            return (abs % 10) switch
            {
                1 when abs % 100 != 11 => 0,
                _ => 1,
            };
        }

        public static int Indonesian(int number) => NoPlurals(number);
        public static int Italian(int number) => NonOneIsPlural(number);
        public static int Japanese(int number) => NoPlurals(number);
        public static int Korean(int number) => NoPlurals(number);

        public static int Latvian(int number)
        {
            var abs = Mathf.Abs(number);
            if (abs % 10 == 1 && abs % 100 != 11)
                return 0;
            return abs != 0 ? 1 : 2;
        }

        public static int Lithuanian(int number)
        {
            var abs = Mathf.Abs(number);
            return (abs % 10) switch
            {
                1 when abs % 100 != 11 => 0,
                >= 2 when abs % 100 < 10 || abs % 100 >= 20 => 1,
                _ => 2,
            };
        }

        public static int Norwegian(int number) => NonOneIsPlural(number);

        public static int Polish(int number)
        {
            var abs = Mathf.Abs(number);

            if (abs == 1)
                return 0;

            return (abs % 10) switch
            {
                2 when abs % 100 != 12 => 1,
                3 when abs % 100 != 13 => 1,
                4 when abs % 100 != 14 => 1,
                _ => 2,
            };
        }

        public static int Portuguese(int number) => NonOneIsPlural(number);

        public static int Romanian(int number)
        {
            var abs = Mathf.Abs(number);
            if (abs == 1)
                return 0;

            if (abs == 0 || (abs % 100 > 0 && abs % 100 < 20))
                return 1;

            return 2;
        }

        public static int Russian(int number) => SouthAndEastSlavic(number);
        public static int SerboCroatian(int number) => SouthAndEastSlavic(number);

        public static int Slovak(int number)
        {
            var abs = Mathf.Abs(number);
            return abs switch
            {
                1 => 0,
                2 => 1,
                3 => 1,
                4 => 1,
                _ => 2,
            };
        }

        public static int Slovenian(int number)
        {
            var n = Mathf.Abs(number);
            return (n % 100) switch
            {
                1 => 0,
                2 => 1,
                3 => 2,
                4 => 2,
                _ => 3,
            };
        }

        public static int Spanish(int number) => NonOneIsPlural(number);
        public static int Swedish(int number) => NonOneIsPlural(number);
        public static int Thai(int number) => NoPlurals(number);
        public static int Turkish(int number) => MoreThanOneIsPlural(number);
        public static int Ukrainian(int number) => SouthAndEastSlavic(number);
        public static int Vietnamese(int number) => NoPlurals(number);
        public static int ChineseSimplified(int number) => NoPlurals(number);
        public static int ChineseTraditional(int number) => NoPlurals(number);
        public static int Hindi(int number) => NonOneIsPlural(number);
    }
}