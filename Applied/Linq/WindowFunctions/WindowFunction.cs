using System.Applied;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq.WindowFunctions;

namespace System.Linq
{
    public static partial class EnumerableWindowFunction
    {
        public static IPartitionedEnumerable<TSource> Partition<TSource>(IEnumerable<TSource> source)
        {
            return Partition(source, p => p);
        }
        public static IPartitionedEnumerable<TSource> Partition<TSource>(IEnumerable<TSource> source
            , Func<IKeepEnumerable<TSource>, IKeepEnumerable<TSource>> orderBySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            Lazy<IKeepEnumerable<TSource>> keep = new Lazy<IKeepEnumerable<TSource>>(() => orderBySelector(new KeepOrderedEnumerable<TSource>()));
            return new Partitioned<TSource>(new IRankEnumerable<TSource>[]
            {
                source.OrderBy(keep.Value, out Func<int[]> keepDenseRank).AsRankEnumerable(keepDenseRank)
            });
        }
        public static IPartitionedEnumerable<TSource> AsPartition<TKey, TSource>(this IEnumerable<IGrouping<TKey, TSource>> source)
        {
            return source.AsPartition(p => p);
        }
        public static IPartitionedEnumerable<TSource> AsPartition<TKey, TSource>(this IEnumerable<IGrouping<TKey, TSource>> source
             , Func<IKeepEnumerable<TSource>, IKeepEnumerable<TSource>> orderBySelector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            Lazy<IKeepEnumerable<TSource>> keep = new Lazy<IKeepEnumerable<TSource>>(() => orderBySelector(new KeepOrderedEnumerable<TSource>()));
            return new Partitioned<TSource>(source.Select(p => p.OrderBy(keep.Value, out Func<int[]> keepDenseRank).AsRankEnumerable(keepDenseRank)));
        }
        private static IRankEnumerable<TSource> AsRankEnumerable<TSource>(this IEnumerable<TSource> source, Func<int[]> keepDenseRank)
        {
            return new RankEnumerable<TSource>(source, keepDenseRank);
        }
        public static IPartitionedEnumerable<TResult> Over<TSource, TElement, TResult>(this IPartitionedEnumerable<TSource> source
            , Func<IEnumerable<TSource>, TElement> aggregate
            , Func<TSource, TElement, TResult> selector)
        {
            Lazy<AggregateFunction<TElement>> function = new Lazy<AggregateFunction<TElement>>(() => new AggregateFunction<TElement>());
            return source.Over(p => function.Value.GetPartitionResults(p, aggregate, selector));
        }
        public static IPartitionedEnumerable<TResult> Over<TSourceBase, TSource, TElement, TResult>(this IPartitionedEnumerable<TSource> source
            , Func<IWindowFunctionFactory<TSource>, IWindowFunction<TSourceBase, TElement>> functionConstructor
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            Lazy<IWindowFunction<TSourceBase, TElement>> function = new Lazy<IWindowFunction<TSourceBase, TElement>>(() => functionConstructor(new WindowFunctionFactory<TSource>()));
            return source.Over(p => function.Value.GetPartitionResults(p, selector));
        }
        private static IPartitionedEnumerable<TResult> Over<TSource, TResult>(this IPartitionedEnumerable<TSource> source
            , Func<IRankEnumerable<TSource>, IEnumerable<TResult>> func)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            return new Partitioned<TResult>(source.Partitions.Select(p => func(p).AsRankEnumerable(p.KeepDenseRank)));
        }
        public static IPartitionedEnumerable<TSource> Over<TSource, TElement>(this IPartitionedEnumerable<TSource> source
            , Func<IEnumerable<TSource>, TElement> aggregate
            , Expression<Func<TSource, TElement>> property) where TSource : class
        {
            return source.Over(aggregate, SelectorOf(property));
        }
        public static IPartitionedEnumerable<TSource> Over<TSourceBase, TSource, TElement>(this IPartitionedEnumerable<TSource> source
            , Func<IWindowFunctionFactory<TSource>, IWindowFunction<TSourceBase, TElement>> functionConstructor
            , Expression<Func<TSource, TElement>> property) where TSource : class, TSourceBase
        {
            return source.Over(functionConstructor, SelectorOf(property));
        }
        private static Func<TSource, TElement, TSource> SelectorOf<TSource, TElement>(Expression<Func<TSource, TElement>> property) where TSource : class
        {
            string[] properties = new string[] { GetPropertyName(property) };
            return (a, value) => a.Apply(() => properties.ToDictionary(n => n, n => value));
        }
        private static string GetPropertyName<TSource, TElement>(Expression<Func<TSource, TElement>> property)
        {
            if (property.Body is MemberExpression memberExpr1)
            {
                return memberExpr1.Member.Name;
            }
            else if (property.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression memberExpr2)
            {
                return memberExpr2.Member.Name;
            }
            else
            {
                throw new ArgumentException("property");
            }
        }
    }
}
namespace System.Linq.WindowFunctions
{
    public interface IRankEnumerable<TSource> : IEnumerable<TSource>
    {
        Func<int[]> KeepDenseRank { get; }
    }
    public interface IPartitionedEnumerable<TSource> : IEnumerable<TSource>
    {
        IEnumerable<IRankEnumerable<TSource>> Partitions { get; }
    }
    public interface IWindowFunctionFactory<TSource>
    {

    }
    public interface IWindowFunction<TSourceBase, TElement>
    {
        IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase;
    }
    public interface IWindowFunction<TElement> : IWindowFunction<object, TElement>
    {

    }
    internal abstract class LazyEnumerableBase<TSource>
    {
        private readonly Lazy<List<TSource>> _list;
        internal protected IEnumerable<TSource> Value
        {
            get
            {
                return _list.Value;
            }
        }
        public LazyEnumerableBase(IEnumerable<TSource> source)
        {
            _list = new Lazy<List<TSource>>(() => source.ToList());
        }
    }
    internal class RankEnumerable<TSource> : LazyEnumerableBase<TSource>, IRankEnumerable<TSource>
    {
        public Func<int[]> KeepDenseRank { get; }
        public RankEnumerable(IEnumerable<TSource> source, Func<int[]> keepDenseRank)
            : base(source)
        {
            this.KeepDenseRank = keepDenseRank;
        }
        public IEnumerator<TSource> GetEnumerator()
        {
            return this.Value.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    internal class Partitioned<TSource> : LazyEnumerableBase<IRankEnumerable<TSource>>, IPartitionedEnumerable<TSource>
    {
        public IEnumerable<IRankEnumerable<TSource>> Partitions
        {
            get
            {
                return this.Value;
            }
        }
        public Partitioned(IEnumerable<IRankEnumerable<TSource>> partitions)
            : base(partitions)
        {

        }
        public IEnumerator<TSource> GetEnumerator()
        {
            return this.Value.SelectMany(a => a).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    internal class AggregateFunction<TElement>
    {
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<IEnumerable<TSource>, TElement> aggregate
            , Func<TSource, TElement, TResult> selector)
        {
            List<TSource> list = new List<TSource>();
            Queue<TSource> queue = new Queue<TSource>();
            TElement value;
            int index = -1;
            int[] keepDenseRank = elements.KeepDenseRank();
            int denseRank;
            int temp = 1;
            foreach (TSource element in elements)
            {
                index += 1;
                denseRank = keepDenseRank[index];
                if (temp != denseRank)
                {
                    temp = denseRank;
                    value = aggregate(list);
                    while (queue.Count > 0)
                    {
                        yield return selector(queue.Dequeue(), value);
                    }
                }
                list.Add(element);
                queue.Enqueue(element);
            }
            value = aggregate(list);
            while (queue.Count > 0)
            {
                yield return selector(queue.Dequeue(), value);
            }
        }
    }
    internal class WindowFunctionFactory<TSource> : IWindowFunctionFactory<TSource>
    {

    }
}
