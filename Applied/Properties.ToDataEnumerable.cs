using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Applied;

namespace System
{
    public static partial class Properties
    {
        public static IEnumerable<T> ToDataEnumerable<TSource, T>(this IEnumerable<TSource> items)
            where TSource : class
            where T : class, new()
        {
            if (items != null)
            {
                TSource temp = null;
                Lazy<MatchProperty[]> matches = new Lazy<MatchProperty[]>(() =>
                {
                    Type type = typeof(T);
                    Type sourceType = typeof(TSource);
                    return (from a in GetProperties(type, type.IsNeedGetter() ? new Lazy(() => new T()) : null)
                            join b in GetProperties(sourceType, sourceType.IsNeedGetter() ? new Lazy(() => temp) : null) on new { Name = a.Name, Type = GetMatchType(a.PropertyType) } equals new { Name = b.Name, Type = GetMatchType(b.PropertyType) }
                            where a.IsReadOnly == false
                            select new MatchProperty()
                            {
                                Right = a,
                                Left = b
                            }).ToArray();
                });
                foreach (TSource a in items)
                {
                    temp = a;
                    yield return ToEntity<TSource, T>(a, matches.Value);
                }
            }
        }
        private static T ToEntity<TSource, T>(this TSource item, MatchProperty[] matches)
            where T : class, new()
        {
            T entity = new T();
            object value;
            bool done;
            foreach (MatchProperty match in matches)
            {
                value = match.Left.GetValue(item);
                if (match.Right.PropertyType != match.Left.PropertyType)
                {
                    value = Convert(value, match.Right.PropertyType, out done);
                }
                match.Right.SetValue(entity, value);
            }
            return entity;
        }
        public static IEnumerable<T> ToDataEnumerable<T>(this IEnumerable<Dictionary<string, object>> dictionaries)
            where T : class, new()
        {
            return ToDataEnumerable<Dictionary<string, object>, T>(dictionaries);
        }
        public static IEnumerable<T> ToDataEnumerable<T>(this DataTable table)
            where T : class, new()
        {
            return ToDataEnumerable<DataRow, T>(table != null ? table.Select() : null);
        }
    }
}
