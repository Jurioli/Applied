using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace System
{
    public static partial class Properties
    {
        private class ApplyNecessary<TSource, T> : MatchesNecessary<ItemValuePair<TSource, T>> where TSource : class
        {
            private readonly PropertyDescriptorsInfo _left;
            private readonly PropertyDescriptorsInfo _right;
            public ApplyNecessary()
            {
                _left = new PropertyDescriptorsInfo(typeof(TSource), this);
                _right = new PropertyDescriptorsInfo(typeof(T), this);
            }
            public override void LoadProperties(IEnumerable<ItemValuePair<TSource, T>> items)
            {
                _left.LoadProperties(items.Select(a => a.Item));
                _right.LoadProperties(items.Select(a => a.Value));
            }
            protected override MatchProperty[] GetReady(IEnumerable<ItemValuePair<TSource, T>> items)
            {
                return JoinMatches(_left, _right, this, items);
            }
        }
        private static void Apply(this object source, MatchProperty[] matches, object newValue)
        {
            object value;
            foreach (MatchProperty match in matches)
            {
                value = match.Right.GetValue(newValue);
                if (match.Left.PropertyType != match.Right.PropertyType)
                {
                    value = Convert(value, match.Left.PropertyType, out bool done);
                    if (!done)
                    {
                        value = GetTypedNull(match.Left.PropertyType);
                    }
                }
                match.Left.SetValue(source, value);
            }
        }
        private static IEnumerable<ItemValuePair<TSource, T>> GetItemValuePairs<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> getter) where TSource : class
        {
            foreach (TSource item in source)
            {
                if (item != null)
                {
                    T value = getter(item);
                    if (value != null)
                    {
                        yield return new ItemValuePair<TSource, T>(item, value);
                    }
                }
            }
        }
        public static IEnumerable<TSource> Apply<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> getter) where TSource : class
        {
            if (source != null)
            {
                ApplyNecessary<TSource, T> necessary = new ApplyNecessary<TSource, T>();
                foreach (ItemValuePair<TSource, T> pair in necessary.Each(source.GetItemValuePairs(getter)))
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
        private class ItemValuePair<TSource, T>
        {
            public TSource Item { get; }
            public T Value { get; }
            public ItemValuePair(TSource item, T value)
            {
                this.Item = item;
                this.Value = value;
            }
        }
    }
}
