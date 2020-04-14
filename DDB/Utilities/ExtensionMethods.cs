using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DartsDiscordBots.Utilities
{
	public static class ExtensionMethods
	{
		public static string ToPascalCase(this string value)
		{
			return char.ToUpper(value[0]) + value.Substring(1).ToLower();
		}
        private static Random _random = new Random();

        public static T GetRandom<T>(this IList<T> items)
        {
            return items[_random.Next(items.Count)];
        }

        public static void Shuffle<T>(this IList<T> items)
        {
            int itemsCount = items.Count;
            for (int i = 0; i < itemsCount; i++)
            {
                int r = i + _random.Next(itemsCount - i);
                T tempItem = items[r];
                items[r] = items[i];
                items[i] = tempItem;
            }
        }

        public static string ToPascalPipeSeparatedString(this IList<string> list)
        {
            return String.Join("|", list.Select(s => s.ToPascalCase()));
        }
    }
}
