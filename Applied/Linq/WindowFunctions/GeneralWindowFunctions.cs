﻿using System.Collections.Generic;
using System.Linq.WindowFunctions;

namespace System.Linq
{
    public static partial class GeneralWindowFunctions
    {
        public static IWindowFunction<int> RowNumber<TSource>(this IWindowFunctionFactory<TSource> factory)
        {
            return new RowNumber();
        }
        public static IWindowFunction<int> Ntile<TSource>(this IWindowFunctionFactory<TSource> factory
            , int rows)
        {
            return new Ntile(rows);
        }
        public static IWindowFunction<int> DenseRank<TSource>(this IWindowFunctionFactory<TSource> factory)
        {
            return new DenseRank();
        }
        public static IWindowFunction<int> Rank<TSource>(this IWindowFunctionFactory<TSource> factory)
        {
            return new Rank();
        }
        public static IWindowFunction<TSource, TElement> FirstValue<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , Func<TSource, TElement> field)
        {
            return new FirstValue<TSource, TElement>(field);
        }
        public static IWindowFunction<TSource, TElement> LastValue<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , Func<TSource, TElement> field)
        {
            return new LastValue<TSource, TElement>(field);
        }
        public static IWindowFunction<TSource, TElement> NthValue<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , Func<TSource, TElement> field, int offset)
        {
            return new NthValue<TSource, TElement>(field, offset);
        }
        public static IWindowFunction<TSource, TElement> Lead<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , Func<TSource, TElement> field, int offset = 1, TElement defaultValue = default(TElement))
        {
            return new Lead<TSource, TElement>(field, offset, defaultValue);
        }
        public static IWindowFunction<TSource, TElement> Lag<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , Func<TSource, TElement> field, int offset = 1, TElement defaultValue = default(TElement))
        {
            return new Lag<TSource, TElement>(field, offset, defaultValue);
        }
        public static IWindowFunction<decimal> CumeDist<TSource>(this IWindowFunctionFactory<TSource> factory)
        {
            return new CumeDist();
        }
        public static IWindowFunction<decimal> PercentRank<TSource>(this IWindowFunctionFactory<TSource> factory)
        {
            return new PercentRank();
        }
        public static IWindowFunction<TSource, TElement> PercentileDisc<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , decimal numeric, Func<TSource, TElement> field)
        {
            return new PercentileDisc<TSource, TElement>(numeric, field);
        }
        public static IWindowFunction<TSource, decimal?> PercentileCont<TSource>(this IWindowFunctionFactory<TSource> factory
            , decimal numeric, Func<TSource, decimal?> field)
        {
            return new PercentileCont<TSource>(numeric, field);
        }
        public static IWindowFunction<TSource, TElement> KeepDenseRankFirst<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , Func<IEnumerable<TSource>, TElement> aggregate)
        {
            return new KeepDenseRankFirst<TSource, TElement>(aggregate);
        }
        public static IWindowFunction<TSource, TElement> KeepDenseRankLast<TSource, TElement>(this IWindowFunctionFactory<TSource> factory
            , Func<IEnumerable<TSource>, TElement> aggregate)
        {
            return new KeepDenseRankLast<TSource, TElement>(aggregate);
        }
    }
}
namespace System.Linq.WindowFunctions
{
    internal class RowNumber : IWindowFunction<int>
    {
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, int, TResult> selector)
        {
            int rowNumber = 0;
            foreach (TSource element in elements)
            {
                rowNumber += 1;
                yield return selector(element, rowNumber);
            }
        }
    }
    internal class Ntile : IWindowFunction<int>
    {
        private readonly int _rows;
        public Ntile(int rows)
        {
            if (rows < 1)
            {
                throw new ArgumentOutOfRangeException("rows");
            }
            _rows = rows;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, int, TResult> selector)
        {
            int temp = _rows;
            int ntile = 1;
            foreach (TSource element in elements)
            {
                if (temp == 0)
                {
                    ntile += 1;
                    temp = _rows;
                }
                temp -= 1;
                yield return selector(element, ntile);
            }
        }
    }
    internal class DenseRank : IWindowFunction<int>
    {
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, int, TResult> selector)
        {
            int index = -1;
            int[] keepDenseRank = elements.KeepDenseRank();
            int denseRank;
            foreach (TSource element in elements)
            {
                index += 1;
                denseRank = keepDenseRank[index];
                yield return selector(element, denseRank);
            }
        }
    }
    internal class Rank : IWindowFunction<int>
    {
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, int, TResult> selector)
        {
            int index = -1;
            int[] keepDenseRank = elements.KeepDenseRank();
            int denseRank;
            int temp = 1, dank = 1;
            foreach (TSource element in elements)
            {
                index += 1;
                denseRank = keepDenseRank[index];
                if (temp != denseRank)
                {
                    temp = denseRank;
                    dank = keepDenseRank.Where(a => a < temp).Count() + 1;
                }
                yield return selector(element, dank);
            }
        }
    }
    internal class FirstValue<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly Func<TSourceBase, TElement> _field;
        public FirstValue(Func<TSourceBase, TElement> field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _field = field;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            TSource firstElement = elements.First();
            TElement firstValue = _field(firstElement);
            foreach (TSource element in elements)
            {
                yield return selector(element, firstValue);
            }
        }
    }
    internal class LastValue<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly Func<TSourceBase, TElement> _field;
        public LastValue(Func<TSourceBase, TElement> field)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _field = field;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            TSource lastElement = elements.Last();
            TElement lastValue = _field(lastElement);
            foreach (TSource element in elements)
            {
                yield return selector(element, lastValue);
            }
        }
    }
    internal class NthValue<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly Func<TSourceBase, TElement> _field;
        private readonly int _offset;
        public NthValue(Func<TSourceBase, TElement> field, int offset)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            if (offset < 1)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            _field = field;
            _offset = offset;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            TSource nthElement = elements.ElementAtOrDefault(_offset - 1);
            TElement nthValue = nthElement != null ? _field(nthElement) : default(TElement);
            foreach (TSource element in elements)
            {
                yield return selector(element, nthValue);
            }
        }
    }
    internal class Lead<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly Func<TSourceBase, TElement> _field;
        private readonly int _offset;
        private readonly TElement _defaultValue;
        public Lead(Func<TSourceBase, TElement> field, int offset, TElement defaultValue)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            if (offset < 1)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            _field = field;
            _offset = offset;
            _defaultValue = defaultValue;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            Queue<TSource> queue = new Queue<TSource>();
            foreach (TSource element in elements)
            {
                if (queue.Count < _offset)
                {
                    queue.Enqueue(element);
                }
                else
                {
                    queue.Enqueue(element);
                    yield return selector(queue.Dequeue(), _field(element));
                }
            }
            while (queue.Count > 0)
            {
                yield return selector(queue.Dequeue(), _defaultValue);
            }
        }
    }
    internal class Lag<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly Func<TSourceBase, TElement> _field;
        private readonly int _offset;
        private readonly TElement _defaultValue;
        public Lag(Func<TSourceBase, TElement> field, int offset, TElement defaultValue)
        {
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            if (offset < 1)
            {
                throw new ArgumentOutOfRangeException("offset");
            }
            _field = field;
            _offset = offset;
            _defaultValue = defaultValue;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            Queue<TElement> queue = new Queue<TElement>();
            foreach (TSource element in elements)
            {
                if (queue.Count < _offset)
                {
                    queue.Enqueue(_field(element));
                    yield return selector(element, _defaultValue);
                }
                else
                {
                    queue.Enqueue(_field(element));
                    yield return selector(element, queue.Dequeue());
                }
            }
        }
    }
    internal class CumeDist : IWindowFunction<decimal>
    {
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, decimal, TResult> selector)
        {
            int index = -1;
            int[] keepDenseRank = elements.KeepDenseRank();
            int denseRank;
            int temp = 1;
            decimal dank = 1.0m, count = elements.Count();
            decimal cumeDist = dank / count;
            foreach (TSource element in elements)
            {
                index += 1;
                denseRank = keepDenseRank[index];
                if (temp != denseRank)
                {
                    temp = denseRank;
                    dank = keepDenseRank.Where(a => a < temp).Count() + 1.0m;
                    cumeDist = dank / count;
                }
                yield return selector(element, cumeDist);
            }
        }
    }
    internal class PercentRank : IWindowFunction<decimal>
    {
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, decimal, TResult> selector)
        {
            int index = -1;
            int[] keepDenseRank = elements.KeepDenseRank();
            int denseRank;
            int temp = 1;
            decimal dankMinus = 0.0m, countMinus = elements.Count() - 1.0m;
            decimal percentDist = dankMinus / countMinus;
            foreach (TSource element in elements)
            {
                index += 1;
                denseRank = keepDenseRank[index];
                if (temp != denseRank)
                {
                    temp = denseRank;
                    dankMinus = keepDenseRank.Where(a => a < temp).Count();
                    percentDist = dankMinus / countMinus;
                }
                yield return selector(element, percentDist);
            }
        }
    }
    internal class PercentileDisc<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly decimal _numeric;
        private readonly Func<TSourceBase, TElement> _field;
        public PercentileDisc(decimal numeric, Func<TSourceBase, TElement> field)
        {
            if (numeric < 0.0m || numeric > 1.0m)
            {
                throw new ArgumentOutOfRangeException("numeric");
            }
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _numeric = numeric;
            _field = field;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            int[] keepDenseRank = elements.KeepDenseRank();
            decimal dank = 1.0m, count = elements.Count();
            decimal cumeDist = dank / count;
            int dankMinus, index = -1;
            foreach (int denseRank in keepDenseRank.Distinct())
            {
                dankMinus = keepDenseRank.Where(a => a < denseRank).Count();
                dank = dankMinus + 1.0m;
                cumeDist = dank / count;
                if (cumeDist >= _numeric)
                {
                    index = dankMinus;
                    break;
                }
            }
            TElement percentileDisc = index != -1 ? _field(elements.ElementAt(index)) : default(TElement);
            foreach (TSource element in elements)
            {
                yield return selector(element, percentileDisc);
            }
        }
    }
    internal class PercentileCont<TSourceBase> : IWindowFunction<TSourceBase, decimal?>
    {
        private readonly decimal _numeric;
        private readonly Func<TSourceBase, decimal?> _field;
        public PercentileCont(decimal numeric, Func<TSourceBase, decimal?> field)
        {
            if (numeric < 0.0m || numeric > 1.0m)
            {
                throw new ArgumentOutOfRangeException("numeric");
            }
            if (field == null)
            {
                throw new ArgumentNullException("field");
            }
            _numeric = numeric;
            _field = field;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, decimal?, TResult> selector) where TSource : TSourceBase
        {
            decimal[] values = (from a in elements.Select(a => _field(a))
                                where a != null
                                select a.Value).ToArray();
            decimal? percentileCont;
            if (values.Length > 0)
            {
                decimal count = values.Length;
                decimal rn = 1.0m + (_numeric * (count - 1));
                if (rn % 1.0m == 0.0m)
                {
                    percentileCont = values[(int)rn - 1];
                }
                else
                {
                    decimal ceiling_rn = Math.Ceiling(rn);
                    decimal floor_rn = Math.Floor(rn);
                    decimal frn_value = values[(int)floor_rn - 1];
                    decimal crn_value = values[(int)ceiling_rn - 1];
                    percentileCont = ((ceiling_rn - rn) * frn_value) + ((rn - floor_rn) * crn_value);
                }
            }
            else
            {
                percentileCont = null;
            }
            foreach (TSource element in elements)
            {
                yield return selector(element, percentileCont);
            }
        }
    }
    internal class KeepDenseRankFirst<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly Func<IEnumerable<TSourceBase>, TElement> _aggregate;
        public KeepDenseRankFirst(Func<IEnumerable<TSourceBase>, TElement> aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            _aggregate = aggregate;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            int[] keepDenseRank = elements.KeepDenseRank();
            int firstDenseRank = keepDenseRank[0];
            int firstDenseRankCount = keepDenseRank.Where(a => a == firstDenseRank).Count();
            IEnumerable<TSourceBase> firstDenseRankElements = elements.Take(firstDenseRankCount).Cast<TSourceBase>();
            TElement keepDenseRankFirst = _aggregate(firstDenseRankElements);
            foreach (TSource element in elements)
            {
                yield return selector(element, keepDenseRankFirst);
            }
        }
    }
    internal class KeepDenseRankLast<TSourceBase, TElement> : IWindowFunction<TSourceBase, TElement>
    {
        private readonly Func<IEnumerable<TSourceBase>, TElement> _aggregate;
        public KeepDenseRankLast(Func<IEnumerable<TSourceBase>, TElement> aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException("aggregate");
            }
            _aggregate = aggregate;
        }
        public IEnumerable<TResult> GetPartitionResults<TSource, TResult>(IRankEnumerable<TSource> elements
            , Func<TSource, TElement, TResult> selector) where TSource : TSourceBase
        {
            int[] keepDenseRank = elements.KeepDenseRank();
            int lastDenseRank = keepDenseRank[keepDenseRank.Length - 1];
            int lastDenseRankCount = keepDenseRank.Where(a => a == lastDenseRank).Count();
            IEnumerable<TSourceBase> lastDenseRankElements = elements.Skip(keepDenseRank.Length - lastDenseRankCount).Cast<TSourceBase>();
            TElement keepDenseRankLast = _aggregate(lastDenseRankElements);
            foreach (TSource element in elements)
            {
                yield return selector(element, keepDenseRankLast);
            }
        }
    }
}