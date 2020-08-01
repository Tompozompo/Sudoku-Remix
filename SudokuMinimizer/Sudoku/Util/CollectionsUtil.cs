using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuMinimizer
{
    public static class CollectionsUtil
    {
        private static readonly Random rnd = new Random();

        public static IEnumerable<IEnumerable<T>> SubSetsOf<T>(this IEnumerable<T> source)
        {
            if (!source.Any())
                return Enumerable.Repeat(Enumerable.Empty<T>(), 1);

            var element = source.Take(1);

            var haveNots = SubSetsOf(source.Skip(1));
            var haves = haveNots.Select(set => element.Concat(set));

            return haves.Concat(haveNots);
        }

        public static IEnumerable<IEnumerable<T>> SubSetsOf<T>(this IEnumerable<T> source, int size)
        {
            return SubSetsOf(source).Where(x => x.Count() == size);
        }

        public static T GetRandomElement<T>(this IList<T> options)
        {
            int index = rnd.Next(0, options.Count);
            return options[index];
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rnd.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
