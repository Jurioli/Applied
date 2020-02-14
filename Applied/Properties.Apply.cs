using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace System
{
    public static partial class Properties
    {
        private class ApplyNecessary<TSource> : NecessaryFirst<ItemValuePair<TSource>, MatchProperty[]> where TSource : class
        {
            private readonly PropertyDescriptorsInfo _left;
            private PropertyDescriptorsInfo _right;
            public ApplyNecessary()
            {
                _left = new PropertyDescriptorsInfo(typeof(TSource), this);
            }
            protected override void First(ItemValuePair<TSource> first)
            {
                _right = new PropertyDescriptorsInfo(first.Value.GetType(), this);
            }
            protected override MatchProperty[] GetReady(IEnumerable<ItemValuePair<TSource>> items)
            {
                return JoinMatches(_left, _right, this.Load, items);
            }
            private void Load(IEnumerable<ItemValuePair<TSource>> items)
            {
                _left.LoadProperties(items.Select(a => a.Item));
                _right.LoadProperties(items.Select(a => a.Value));
            }
        }
        private static void Apply(this object source, MatchProperty[] matches, object newValue)
        {
            foreach (MatchProperty match in matches)
            {
                match.Apply(source, newValue);
            }
        }
        private static IEnumerable<ItemValuePair<TSource>> GetItemValuePairs<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> getter) where TSource : class
        {
            foreach (TSource item in source)
            {
                if (item != null)
                {
                    T value = getter(item);
                    if (value != null)
                    {
                        yield return new ItemValuePair<TSource>(item, value);
                    }
                }
            }
        }
        public static IEnumerable<TSource> Apply<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> getter) where TSource : class
        {
            if (source != null)
            {
                ApplyNecessary<TSource> necessary = new ApplyNecessary<TSource>();
                foreach (ItemValuePair<TSource> pair in necessary.Each(source.GetItemValuePairs(getter)))
                {
                    pair.Item.Apply(necessary.Value, pair.Value);
                }
            }
            return source;
        }
        public static TSource Apply<TSource, T>(this TSource source, Func<T> getter) where TSource : class
        {
            if (source != null)
            {
                new TSource[] { source }.Apply(a => getter());
            }
            return source;
        }
        public static DataTable Apply<T>(this DataTable source, Func<DataRow, T> getter)
        {
            if (source != null)
            {
                source.AsEnumerable().Apply(getter);
            }
            return source;
        }
        private class ItemValuePair<TSource>
        {
            public TSource Item { get; }
            public object Value { get; }
            public ItemValuePair(TSource item, object value)
            {
                this.Item = item;
                this.Value = value;
            }
        }
    }
}
