using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;

namespace System
{
    public static partial class Properties
    {
        private class EntityNecessary<TSource, T> : MatchesNecessary<TSource>
        {
            private readonly PropertyDescriptorsInfo _left;
            private readonly PropertyDescriptorsInfo _right;
            public EntityNecessary()
            {
                _left = new PropertyDescriptorsInfo(typeof(T), PropertyDescriptorKind.Class);
                _right = new PropertyDescriptorsInfo(typeof(TSource), this);
            }
            public override void LoadProperties(IEnumerable<TSource> items)
            {
                _left.LoadEntityProperties();
                _right.LoadProperties(items);
            }
            protected override MatchProperty[] GetReady(IEnumerable<TSource> items)
            {
                return JoinMatches(_left, _right, this, items);
            }
        }
        private static void LoadEntityProperties(this PropertyDescriptorsInfo info)
        {
            info.Properties = GetPropertyDescriptors(info.Type);
        }
        private static T ToEntity<TSource, T>(this TSource item, MatchProperty[] matches)
            where T : class, new()
        {
            if (item == null)
            {
                return null;
            }
            T entity = new T();
            object value;
            foreach (MatchProperty match in matches)
            {
                value = match.Right.GetValue(item);
                if (match.Left.PropertyType != match.Right.PropertyType)
                {
                    value = Convert(value, match.Left.PropertyType, out bool done);
                    if (!done)
                    {
                        value = GetTypedNull(match.Left.PropertyType);
                    }
                }
                match.Left.SetValue(entity, value);
            }
            return entity;
        }
        public static IEnumerable<T> ToDataEnumerable<TSource, T>(this IEnumerable<TSource> items)
            where TSource : class
            where T : class, new()
        {
            if (items != null)
            {
                EntityNecessary<TSource, T> necessary = new EntityNecessary<TSource, T>();
                foreach (TSource item in necessary.Each(items))
                {
                    yield return item.ToEntity<TSource, T>(necessary.Value);
                }
            }
        }
        public static IEnumerable<T> ToDataEnumerable<T>(this IEnumerable<Dictionary<string, object>> dictionaries)
            where T : class, new()
        {
            return ToDataEnumerable<Dictionary<string, object>, T>(dictionaries);
        }
        private static IEnumerable<DataRow> AsEnumerable(this DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                yield return row;
            }
        }
        public static IEnumerable<T> ToDataEnumerable<T>(this DataTable table)
            where T : class, new()
        {
            return ToDataEnumerable<DataRow, T>(table?.AsEnumerable());
        }
    }
}
