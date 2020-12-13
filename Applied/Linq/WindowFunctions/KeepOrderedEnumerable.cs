using System.Applied;
using System.Collections.Generic;
using System.Linq.WindowFunctions;

namespace System.Linq
{
    public static partial class EnumerableWindowFunction
    {
        public static IKeepOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IKeepEnumerable<TSource> source
            , Func<TSource, TKey> keySelector)
        {
            return new KeepOrderedEnumerable<TSource>(source).Add(keySelector);
        }
        public static IKeepOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IKeepEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new KeepOrderedEnumerable<TSource>(source).Add(keySelector, comparer);
        }
        public static IKeepOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IKeepEnumerable<TSource> source
            , Func<TSource, TKey> keySelector)
        {
            return new KeepOrderedEnumerable<TSource>(source).Add(keySelector, descending: true);
        }
        public static IKeepOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IKeepEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return new KeepOrderedEnumerable<TSource>(source).Add(keySelector, comparer, true);
        }
        public static IKeepOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IKeepOrderedEnumerable<TSource> source
            , Func<TSource, TKey> keySelector)
        {
            return source.OrderBy(keySelector);
        }
        public static IKeepOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IKeepOrderedEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return source.OrderBy(keySelector, comparer);
        }
        public static IKeepOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IKeepOrderedEnumerable<TSource> source
            , Func<TSource, TKey> keySelector)
        {
            return source.OrderByDescending(keySelector);
        }
        public static IKeepOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IKeepOrderedEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            return source.OrderByDescending(keySelector, comparer);
        }
        public static IKeepOrderedEnumerable<TSource> OrderBy<TSource, TKey>(this IKeepEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.OrderBy(keySelector, new LambdaComparer<TKey>(comparer));
        }
        public static IKeepOrderedEnumerable<TSource> OrderByDescending<TSource, TKey>(this IKeepEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.OrderByDescending(keySelector, new LambdaComparer<TKey>(comparer));
        }
        public static IKeepOrderedEnumerable<TSource> ThenBy<TSource, TKey>(this IKeepOrderedEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.ThenBy(keySelector, new LambdaComparer<TKey>(comparer));
        }
        public static IKeepOrderedEnumerable<TSource> ThenByDescending<TSource, TKey>(this IKeepOrderedEnumerable<TSource> source
            , Func<TSource, TKey> keySelector, Func<TKey, TKey, int> comparer)
        {
            return source.ThenByDescending(keySelector, new LambdaComparer<TKey>(comparer));
        }
        private static IEnumerable<TSource> OrderBy<TSource>(this IEnumerable<TSource> source, IKeepEnumerable<TSource> keep
            , out Func<int[]> keepDenseRank)
        {
            foreach (IOrderedParameters<TSource> parameters in keep.Orders.Reverse())
            {
                source = parameters.Sort(source);
            }
            keepDenseRank = source.GetKeepDenseRank(keep.Orders);
            return source;
        }
        private static Func<int[]> GetKeepDenseRank<TSource>(this IEnumerable<TSource> source, IEnumerable<IOrderedParameters<TSource>> orders)
        {
            Lazy<int[]> keepDenseRank = new Lazy<int[]>(() =>
            {
                if (orders.Any())
                {
                    object[] getKeys(TSource a) => orders.Select(p => p.GetValue(a)).ToArray();
                    bool compareKeys(object[] x, object[] y) => x.Select((a, i) => Equals(a, y[i])).All(a => a);
                    return source.GroupBy(getKeys, compareKeys).SelectMany((g, i) =>
                    {
                        i += 1;
                        return g.Select(a => i);
                    }).ToArray();
                }
                else
                {
                    return source.Select(a => 1).ToArray();
                }
            });
            return () => keepDenseRank.Value;
        }
    }
}
namespace System.Linq.WindowFunctions
{
    public interface IKeepEnumerable<TSource>
    {
        IEnumerable<IOrderedParameters<TSource>> Orders { get; }
    }
    public interface IKeepOrderedEnumerable<TSource> : IKeepEnumerable<TSource>
    {

    }
    public interface IOrderedParameters<TSource>
    {
        IEnumerable<TSource> Sort(IEnumerable<TSource> source);
        object GetValue(TSource source);
    }
    internal class KeepOrderedEnumerable<TSource> : IKeepOrderedEnumerable<TSource>
    {
        private readonly List<IOrderedParameters<TSource>> _orders;
        public IEnumerable<IOrderedParameters<TSource>> Orders
        {
            get
            {
                return _orders;
            }
        }
        public KeepOrderedEnumerable()
        {
            _orders = new List<IOrderedParameters<TSource>>();
        }
        public KeepOrderedEnumerable(IKeepEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            _orders = new List<IOrderedParameters<TSource>>(source.Orders ?? Enumerable.Empty<IOrderedParameters<TSource>>());
        }
        public KeepOrderedEnumerable<TSource> Add<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer = null, bool descending = false)
        {
            _orders.Add(new OrderedParameters<TKey>()
            {
                KeySelector = keySelector,
                Comparer = comparer,
                Descending = descending
            });
            return this;
        }
        private class OrderedParameters<TKey> : IOrderedParameters<TSource>
        {
            public Func<TSource, TKey> KeySelector { get; set; }
            public IComparer<TKey> Comparer { get; set; }
            public bool Descending { get; set; }
            public IEnumerable<TSource> Sort(IEnumerable<TSource> source)
            {
                if (this.Descending)
                {
                    return source.OrderByDescending(this.KeySelector, this.Comparer);
                }
                else
                {
                    return source.OrderBy(this.KeySelector, this.Comparer);
                }
            }
            public object GetValue(TSource source)
            {
                return this.KeySelector(source);
            }
        }
    }
}
