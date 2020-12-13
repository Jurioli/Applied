using System.Applied;
using System.Collections;
using System.Collections.Generic;
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
        public static IPartitionedEnumerable<TResult> Over<TSource, IElement, TResult>(this IPartitionedEnumerable<TSource> source
            , Func<IEnumerable<TSource>, IElement> aggregate
            , Func<TSource, IElement, TResult> selector)
        {
            Lazy<AggregateFunction<IElement>> function = new Lazy<AggregateFunction<IElement>>(() => new AggregateFunction<IElement>());
            return source.Over(p => function.Value.GetPartitionResults(p, aggregate, selector));
        }
        public static IPartitionedEnumerable<TResult> Over<TSourceBase, TSource, IElement, TResult>(this IPartitionedEnumerable<TSource> source
            , Func<IWindowFunctionFactory<TSource>, IWindowFunction<TSourceBase, IElement>> functionConstructor
            , Func<TSource, IElement, TResult> selector) where TSource : TSourceBase
        {
            Lazy<IWindowFunction<TSourceBase, IElement>> function = new Lazy<IWindowFunction<TSourceBase, IElement>>(() => functionConstructor(new WindowFunctionFactory<TSource>()));
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
    public interface IWindowFunction<TSourceBase, IElement>
    {
        IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, IElement, TResult> selector) where TSource : TSourceBase;
    }
    public interface IWindowFunction<IElement> : IWindowFunction<object, IElement>
    {

    }
    internal class RankEnumerable<TSource> : IRankEnumerable<TSource>
    {
        private readonly IEnumerable<TSource> _source;
        private readonly Lazy<List<TSource>> _list;
        public Func<int[]> KeepDenseRank { get; }
        public RankEnumerable(IEnumerable<TSource> source, Func<int[]> keepDenseRank)
        {
            _source = source;
            _list = new Lazy<List<TSource>>(() => _source.ToList());
            this.KeepDenseRank = keepDenseRank;
        }
        public IEnumerator<TSource> GetEnumerator()
        {
            return _list.Value.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    internal class Partitioned<TSource> : IPartitionedEnumerable<TSource>
    {
        private readonly IEnumerable<IRankEnumerable<TSource>> _partitions;
        private readonly Lazy<List<IRankEnumerable<TSource>>> _list;
        public IEnumerable<IRankEnumerable<TSource>> Partitions
        {
            get
            {
                return _list.Value;
            }
        }
        public Partitioned(IEnumerable<IRankEnumerable<TSource>> partitions)
        {
            _partitions = partitions;
            _list = new Lazy<List<IRankEnumerable<TSource>>>(() => _partitions.ToList());
        }
        public IEnumerator<TSource> GetEnumerator()
        {
            return _list.Value.SelectMany(a => a).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    internal class AggregateFunction<IElement>
    {
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<IEnumerable<TSource>, IElement> aggregate
            , Func<TSource, IElement, TResult> selector)
        {
            List<TSource> list = new List<TSource>();
            Queue<TSource> queue = new Queue<TSource>();
            IElement value;
            int index = -1;
            int[] deepDenseRank = elements.KeepDenseRank();
            int denseRank;
            int temp = 1;
            foreach (TSource element in elements)
            {
                index += 1;
                denseRank = deepDenseRank[index];
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
