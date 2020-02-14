using System.Collections.Generic;

namespace System.Linq
{
    public class LambdaComparer<TKey> : IComparer<TKey>
    {
        private readonly Func<TKey, TKey, int> _comparer;
        public LambdaComparer(Func<TKey, TKey, int> comparer)
        {
            _comparer = comparer;
        }
        public int Compare(TKey x, TKey y)
        {
            return _comparer(x, y);
        }
    }
    public static partial class EnumerableLambda
    {
        public static IOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.OrderBy(keySelector, new LambdaComparer<TKey>(comparer));
        }
        public static IOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.OrderByDescending(keySelector, new LambdaComparer<TKey>(comparer));
        }
        public static IOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.ThenBy(keySelector, new LambdaComparer<TKey>(comparer));
        }
        public static IOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IOrderedEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.ThenByDescending(keySelector, new LambdaComparer<TKey>(comparer));
        }
    }
}
