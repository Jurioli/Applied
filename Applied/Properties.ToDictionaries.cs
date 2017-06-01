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
        public static IEnumerable<Dictionary<string, object>> ToDictionaries<T>(this IEnumerable<T> items)
            where T : class
        {
            return ToDictionaries<T, Dictionary<string, object>>(items);
        }
        public static IEnumerable<TDictionary> ToDictionaries<T, TDictionary>(this IEnumerable<T> items)
            where TDictionary : IDictionary, new()
            where T : class
        {
            if (items != null)
            {
                T temp = null;
                Lazy<MatchProperty[]> matches = new Lazy<MatchProperty[]>(() =>
                {
                    Type type = typeof(T);
                    return (from a in GetProperties(type, type.IsNeedGetter() ? new Lazy(() => temp) : null)
                            select new MatchProperty()
                            {
                                Right = a,
                                Left = new DictionaryPropertyDescriptor(a.Name, a.PropertyType)
                            }).ToArray();
                });
                foreach (T item in items)
                {
                    temp = item;
                    TDictionary dictionary = new TDictionary();
                    foreach (MatchProperty match in matches.Value)
                    {
                        match.Left.SetValue(dictionary, match.Right.GetValue(item));
                    }
                    yield return dictionary;
                }
            }
        }
        public static IEnumerable<Dictionary<string, object>> ToDictionaries(this DataTable table)
        {
            return ToDictionaries<Dictionary<string, object>>(table);
        }
        public static IEnumerable<TDictionary> ToDictionaries<TDictionary>(this DataTable table)
            where TDictionary : IDictionary, new()
        {
            return ToDictionaries<DataRow, TDictionary>(table != null ? table.Select() : null);
        }
    }
}
