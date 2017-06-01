using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Applied;

namespace System
{
    public static partial class Properties
    {
        private static readonly Dictionary<string, MatchProperty[]> _matchesDictionary = new Dictionary<string, MatchProperty[]>();
        private static MatchProperty[] GetMatches(Type left, Lazy leftGetter, Type right, Lazy rightGetter)
        {
            string leftKey, rightKey;
            Lazy<PropertyDescriptor[]> rightProperties = GetProperties(right, rightGetter, out rightKey);
            Lazy<PropertyDescriptor[]> leftProperties = GetProperties(left, leftGetter, out leftKey);
            if (leftKey != null && rightKey != null)
            {
                string key = leftKey + "#" + rightKey;
                MatchProperty[] matches;
                if (!_matchesDictionary.TryGetValue(key, out matches))
                {
                    lock (_matchesDictionary)
                    {
                        if (!_matchesDictionary.TryGetValue(key, out matches))
                        {
                            matches = (from a in rightProperties.Value
                                       join b in leftProperties.Value on new { Name = a.Name, Type = GetMatchType(a.PropertyType) } equals new { Name = b.Name, Type = GetMatchType(b.PropertyType) }
                                       where b.IsReadOnly == false
                                       select new MatchProperty() { Left = b, Right = a }).ToArray();
                            _matchesDictionary.Add(key, matches);
                        }
                    }
                }
                return matches;
            }
            else
            {
                if (typeof(IDictionary).IsAssignableFrom(left))
                {
                    List<string> list = leftProperties.Value.Select(a => a.Name).ToList();
                    PropertyDescriptor[] newLeftProperties = leftProperties.Value.Concat((from a in rightProperties.Value
                                                                                          where list.Contains(a.Name) == false
                                                                                          select new DictionaryPropertyDescriptor(a.Name, a.PropertyType)).ToArray()).ToArray();
                    return (from a in rightProperties.Value
                            join b in newLeftProperties on new { Name = a.Name, Type = GetMatchType(a.PropertyType) } equals new { Name = b.Name, Type = GetMatchType(b.PropertyType) }
                            where b.IsReadOnly == false
                            select new MatchProperty() { Left = b, Right = a }).ToArray();
                }
                return (from a in rightProperties.Value
                        join b in leftProperties.Value on new { Name = a.Name, Type = GetMatchType(a.PropertyType) } equals new { Name = b.Name, Type = GetMatchType(b.PropertyType) }
                        where b.IsReadOnly == false
                        select new MatchProperty() { Left = b, Right = a }).ToArray();
            }
        }
        public static TSource Apply<TSource, T>(this TSource source, Func<T> getter) where TSource : class
        {
            if (source != null)
            {
                T value = getter();
                if (value != null)
                {
                    Type left = typeof(TSource);
                    Type right = value.GetType();
                    MatchProperty[] matches = GetMatches(left, left.IsNeedGetter() ? new Lazy(() => source) : null
                        , right, right.IsNeedGetter() ? new Lazy(() => value) : null);
                    source.Apply(matches, value);
                    return source;
                }
            }
            return source;
        }
        public static IEnumerable<TSource> Apply<TSource, T>(this IEnumerable<TSource> source, Func<TSource, T> getter) where TSource : class
        {
            if (source != null)
            {
                TSource temp = null;
                T value = default(T);
                Lazy<MatchProperty[]> matches = new Lazy<MatchProperty[]>(() =>
                {
                    Type left = typeof(TSource);
                    Type right = typeof(T);
                    return GetMatches(left, left.IsNeedGetter() ? new Lazy(() => temp) : null
                        , right, right.IsNeedGetter() ? new Lazy(() => value) : null);
                });
                foreach (TSource item in source)
                {
                    temp = item;
                    value = getter(item);
                    if (value != null)
                    {
                        item.Apply(matches.Value, value);
                    }
                }
            }
            return source;
        }
        private static void Apply(this object source, MatchProperty[] matches, object newValue)
        {
            object value;
            bool done;
            foreach (MatchProperty match in matches)
            {
                value = match.Right.GetValue(newValue);
                if (match.Left.PropertyType != match.Right.PropertyType)
                {
                    value = Convert(value, match.Left.PropertyType, out done);
                }
                match.Left.SetValue(source, value);
            }
        }
        public static DataTable Apply<T>(this DataTable source, Func<DataRow, T> getter)
        {
            if (source != null)
            {
                source.Select().Apply(getter);
            }
            return source;
        }
        private class MatchProperty
        {
            public PropertyDescriptor Left { get; set; }
            public PropertyDescriptor Right { get; set; }
        }
    }
}
