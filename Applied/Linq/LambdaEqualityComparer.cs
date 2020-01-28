using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Linq
{
    public class LambdaEqualityComparer<TKey> : IEqualityComparer<TKey>
    {
        private readonly Func<TKey, TKey, bool> _equals;
        private readonly Func<TKey, int> _getHashCode;
        public LambdaEqualityComparer(Func<TKey, TKey, bool> equals) : this(equals, null)
        {

        }
        public LambdaEqualityComparer(Func<TKey, TKey, bool> equals, Func<TKey, int> getHashCode)
        {
            _equals = equals;
            _getHashCode = getHashCode;
        }
        public bool Equals(TKey x, TKey y)
        {
            return _equals(x, y);
        }
        public int GetHashCode(TKey obj)
        {
            return _getHashCode != null ? _getHashCode(obj) : 0;
        }
    }
    public static partial class EnumerableLambda
    {
        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, Func<TSource, TSource, bool> comparer)
        {
            return source.Contains(value, new LambdaEqualityComparer<TSource>(comparer));
        }
        public static bool Contains<TSource>(this IEnumerable<TSource> source, TSource value, Func<TSource, TSource, bool> comparer, Func<TSource, int> getHashCode)
        {
            return source.Contains(value, new LambdaEqualityComparer<TSource>(comparer, getHashCode));
        }
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> comparer)
        {
            return source.Distinct(new LambdaEqualityComparer<TSource>(comparer));
        }
        public static IEnumerable<TSource> Distinct<TSource>(this IEnumerable<TSource> source, Func<TSource, TSource, bool> comparer, Func<TSource, int> getHashCode)
        {
            return source.Distinct(new LambdaEqualityComparer<TSource>(comparer, getHashCode));
        }
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Except(second, new LambdaEqualityComparer<TSource>(comparer));
        }
        public static IEnumerable<TSource> Except<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer, Func<TSource, int> getHashCode)
        {
            return first.Except(second, new LambdaEqualityComparer<TSource>(comparer, getHashCode));
        }
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer)
        {
            return source.GroupBy(keySelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.GroupBy(keySelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Func<TKey, TKey, bool> comparer)
        {
            return source.GroupBy(keySelector, elementSelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.GroupBy(keySelector, elementSelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, Func<TKey, TKey, bool> comparer)
        {
            return source.GroupBy(keySelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static IEnumerable<TResult> GroupBy<TSource, TKey, TResult>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.GroupBy(keySelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> comparer)
        {
            return source.GroupBy(keySelector, elementSelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static IEnumerable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.GroupBy(keySelector, elementSelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TKey, bool> comparer)
        {
            return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return outer.GroupJoin(inner, outerKeySelector, innerKeySelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Intersect(second, new LambdaEqualityComparer<TSource>(comparer));
        }
        public static IEnumerable<TSource> Intersect<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer, Func<TSource, int> getHashCode)
        {
            return first.Intersect(second, new LambdaEqualityComparer<TSource>(comparer, getHashCode));
        }
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TKey, bool> comparer)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return outer.Join(inner, outerKeySelector, innerKeySelector, resultSelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.SequenceEqual(second, new LambdaEqualityComparer<TSource>(comparer));
        }
        public static bool SequenceEqual<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer, Func<TSource, int> getHashCode)
        {
            return first.SequenceEqual(second, new LambdaEqualityComparer<TSource>(comparer, getHashCode));
        }
        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer)
        {
            return source.ToDictionary(keySelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static Dictionary<TKey, TSource> ToDictionary<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.ToDictionary(keySelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> comparer)
        {
            return source.ToDictionary(keySelector, elementSelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.ToDictionary(keySelector, elementSelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer)
        {
            return source.ToLookup(keySelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static ILookup<TKey, TSource> ToLookup<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.ToLookup(keySelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> comparer)
        {
            return source.ToLookup(keySelector, elementSelector, new LambdaEqualityComparer<TKey>(comparer));
        }
        public static ILookup<TKey, TElement> ToLookup<TSource, TKey, TElement>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, TKey, bool> comparer, Func<TKey, int> getHashCode)
        {
            return source.ToLookup(keySelector, elementSelector, new LambdaEqualityComparer<TKey>(comparer, getHashCode));
        }
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer)
        {
            return first.Union(second, new LambdaEqualityComparer<TSource>(comparer));
        }
        public static IEnumerable<TSource> Union<TSource>(this IEnumerable<TSource> first, IEnumerable<TSource> second, Func<TSource, TSource, bool> comparer, Func<TSource, int> getHashCode)
        {
            return first.Union(second, new LambdaEqualityComparer<TSource>(comparer, getHashCode));
        }
    }
}
