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
            return source.Over(aggregate, FrameRange.Default, selector);
        }
        public static IPartitionedEnumerable<TResult> Over<TSource, TElement, TResult>(this IPartitionedEnumerable<TSource> source
            , Func<IEnumerable<TSource>, TElement> aggregate, FrameRange range
            , Func<TSource, TElement, TResult> selector)
        {
            Lazy<AggregateFunction<TElement>> function = new Lazy<AggregateFunction<TElement>>(() => new AggregateFunction<TElement>());
            return source.Over(p => function.Value.GetPartitionResults(p, aggregate, range, selector));
        }
        public static IPartitionedEnumerable<TResult> Over<TSource, TElement, TResult>(this IPartitionedEnumerable<TSource> source
            , Func<IEnumerable<TSource>, TElement> aggregate, FrameRows rows
            , Func<TSource, TElement, TResult> selector)
        {
            Lazy<AggregateFunction<TElement>> function = new Lazy<AggregateFunction<TElement>>(() => new AggregateFunction<TElement>());
            return source.Over(p => function.Value.GetPartitionResults(p, aggregate, rows, selector));
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
            return source.Over(aggregate, FrameRange.Default, SelectorOf(property));
        }
        public static IPartitionedEnumerable<TSource> Over<TSource, TElement>(this IPartitionedEnumerable<TSource> source
            , Func<IEnumerable<TSource>, TElement> aggregate, FrameRange range
            , Expression<Func<TSource, TElement>> property) where TSource : class
        {
            return source.Over(aggregate, range, SelectorOf(property));
        }
        public static IPartitionedEnumerable<TSource> Over<TSource, TElement>(this IPartitionedEnumerable<TSource> source
            , Func<IEnumerable<TSource>, TElement> aggregate, FrameRows rows
            , Expression<Func<TSource, TElement>> property) where TSource : class
        {
            return source.Over(aggregate, rows, SelectorOf(property));
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
            , Func<IEnumerable<TSource>, TElement> aggregate, FrameRange range
            , Func<TSource, TElement, TResult> selector)
        {
            int[] keepDenseRank = elements.KeepDenseRank();
            if (keepDenseRank.Length == 0)
            {
                yield break;
            }
            TSource[][] sources = elements.Select((a, i) => (Rank: keepDenseRank[i], Element: a))
                .GroupBy(a => a.Rank)
                .Select(g => g.Select(a => a.Element).ToArray())
                .ToArray();
            if (range == null)
            {
                range = FrameRange.Default;
            }
            Queue<TSource[]> list = new Queue<TSource[]>();
            FrameBound start = range.Start;
            FrameBound end = range.End;
            TElement value;
            foreach (TSource[] item in this.FrameElements(sources, list, start, end))
            {
                value = aggregate(list.SelectMany(a => a));
                foreach (TSource source in item)
                {
                    yield return selector(source, value);
                }
            }
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<IEnumerable<TSource>, TElement> aggregate, FrameRows rows
            , Func<TSource, TElement, TResult> selector)
        {
            TSource[] sources = elements.ToArray();
            if (sources.Length == 0)
            {
                yield break;
            }
            if (rows == null)
            {
                rows = FrameRows.Default;
            }
            Queue<TSource> list = new Queue<TSource>();
            FrameBound start = rows.Start;
            FrameBound end = rows.End;
            TElement value;
            foreach (TSource item in this.FrameElements(sources, list, start, end))
            {
                value = aggregate(list);
                yield return selector(item, value);
            }
        }
        private IEnumerable<TSource> FrameElements<TSource>(TSource[] sources, Queue<TSource> list, FrameBound start, FrameBound end)
        {
            int lastIndex = sources.Length - 1;
            bool startUnbounded = start.IsUnbounded(out int startOffset);
            bool endUnbounded = end.IsUnbounded(out int endOffset);
            int startIndex = startUnbounded ? start.UnboundedIndex(lastIndex, startOffset) : 0;
            int endIndex = endUnbounded ? end.UnboundedIndex(lastIndex, endOffset) : lastIndex;
            if ((startUnbounded ? startIndex > lastIndex : startOffset > endIndex) ||
                (endUnbounded ? endIndex < 0 : (lastIndex + endOffset) < startIndex) ||
                (startUnbounded && endUnbounded && startIndex > endIndex) ||
                (!startUnbounded && !endUnbounded && startOffset > endOffset))
            {
                foreach (TSource item in sources)
                {
                    yield return item;
                }
            }
            else
            {
                Queue<TSource> queue1 = new Queue<TSource>();
                int index1;
                if (endUnbounded)
                {
                    index1 = -1;
                    if (startUnbounded)
                    {
                        foreach (TSource item in sources)
                        {
                            queue1.Enqueue(item);
                            index1 += 1;
                            if (index1 >= startIndex && index1 <= endIndex)
                            {
                                list.Enqueue(item);
                            }
                        }
                        while (queue1.Count > 0)
                        {
                            yield return queue1.Dequeue();
                        }
                    }
                    else
                    {
                        int index3 = startOffset < 0 ? -1 + startOffset : -1;
                        foreach (TSource item in sources)
                        {
                            queue1.Enqueue(item);
                            index1 += 1;
                            if (index1 >= startOffset && index1 <= endIndex)
                            {
                                list.Enqueue(item);
                            }
                        }
                        while (queue1.Count > 0)
                        {
                            yield return queue1.Dequeue();
                            index3 += 1;
                            if (index3 > -1 && list.Count > 0)
                            {
                                _ = list.Dequeue();
                            }
                        }
                    }
                }
                else
                {
                    Queue<TSource> queue2 = new Queue<TSource>();
                    int index2;
                    if (endOffset < 0)
                    {
                        index1 = -1 + endOffset;
                        index2 = -1;
                    }
                    else
                    {
                        index1 = -1;
                        index2 = -1 - endOffset;
                    }
                    if (startUnbounded)
                    {
                        foreach (TSource item in sources)
                        {
                            queue1.Enqueue(item);
                            queue2.Enqueue(item);
                            index1 += 1;
                            if (index1 > -1)
                            {
                                TSource item1 = queue2.Dequeue();
                                if (index1 >= startIndex)
                                {
                                    list.Enqueue(item1);
                                }
                            }
                            index2 += 1;
                            if (index2 > -1)
                            {
                                yield return queue1.Dequeue();
                            }
                        }
                        while (queue1.Count > 0)
                        {
                            yield return queue1.Dequeue();
                        }
                    }
                    else
                    {
                        int index3 = startOffset < 0 ? -1 + startOffset : -1;
                        foreach (TSource item in sources)
                        {
                            queue1.Enqueue(item);
                            index1 += 1;
                            if (index1 >= startOffset)
                            {
                                queue2.Enqueue(item);
                                if (index1 > -1)
                                {
                                    list.Enqueue(queue2.Dequeue());
                                }
                            }
                            index2 += 1;
                            if (index2 > -1)
                            {
                                yield return queue1.Dequeue();
                                index3 += 1;
                                if (index3 > -1 && list.Count > 0)
                                {
                                    _ = list.Dequeue();
                                }
                            }
                        }
                        while (queue1.Count > 0)
                        {
                            yield return queue1.Dequeue();
                            index3 += 1;
                            if (index3 > -1 && list.Count > 0)
                            {
                                _ = list.Dequeue();
                            }
                        }
                    }
                }
            }
        }
    }
    internal class WindowFunctionFactory<TSource> : IWindowFunctionFactory<TSource>
    {

    }
}
