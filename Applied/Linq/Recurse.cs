using System.Collections.Generic;
using System.Linq;

namespace System.Linq
{
    public static class EnumerableRecurse
    {
        public static IEnumerable<TSource> Recurse<TSource>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TSource>> recurser, int maxRecursion = 100)
        {
            return source.Recurse(recurser, (list, item) => list.Contains(item), maxRecursion);
        }
        public static IEnumerable<TSource> Recurse<TSource>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TSource>> recurser, IEqualityComparer<TSource> comparer, int maxRecursion = 100)
        {
            return source.Recurse(recurser, (list, item) => list.Contains(item, comparer), maxRecursion);
        }
        public static IEnumerable<TSource> Recurse<TSource>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TSource>> recurser, Func<TSource, TSource, bool> comparer, int maxRecursion = 100)
        {
            return source.Recurse(recurser, (list, item) => list.Contains(item, comparer), maxRecursion);
        }
        public static IEnumerable<TSource> Recurse<TSource>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TSource>> recurser, Func<TSource, TSource, bool> comparer, Func<TSource, int> getHashCode, int maxRecursion = 100)
        {
            return source.Recurse(recurser, (list, item) => list.Contains(item, comparer, getHashCode), maxRecursion);
        }
        private static IEnumerable<TSource> Recurse<TSource>(this IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TSource>> recurser, Func<IEnumerable<TSource>, TSource, bool> contains, int maxRecursion)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (recurser == null)
            {
                throw new ArgumentNullException("recurser");
            }
            List<TSource> list;
            int temp;
            int recursion = 0;
            do
            {
                IEnumerable<TSource> items = recurser(source);
                if (items == null || items == source)
                {
                    break;
                }
                list = source.ToList();
                temp = list.Count;
                foreach (TSource item in items)
                {
                    if (!contains(list, item))
                    {
                        list.Add(item);
                    }
                }
                source = list;
                recursion++;
            }
            while (temp != list.Count && (maxRecursion == 0 || recursion < maxRecursion));
            return source;
        }
    }
}
