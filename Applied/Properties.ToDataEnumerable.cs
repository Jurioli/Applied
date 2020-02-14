using System.Collections.Generic;
using System.Data;

namespace System
{
    public static partial class Properties
    {
        private class EntityNecessary<TSource, T> : Necessary<TSource, MatchProperty[]>
        {
            private readonly PropertyDescriptorsInfo _left;
            private readonly PropertyDescriptorsInfo _right;
            public EntityNecessary()
            {
                _left = new PropertyDescriptorsInfo(typeof(T), PropertyDescriptorKind.Class);
                _right = new PropertyDescriptorsInfo(typeof(TSource), this);
            }
            protected override MatchProperty[] GetReady(IEnumerable<TSource> items)
            {
                return JoinMatches(_left, _right, this.Load, items);
            }
            private void Load(IEnumerable<TSource> items)
            {
                _left.LoadEntityProperties();
                _right.LoadProperties(items);
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
            foreach (MatchProperty match in matches)
            {
                match.Apply(entity, item);
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
