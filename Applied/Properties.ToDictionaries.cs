using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace System
{
    public static partial class Properties
    {
        private class DictionaryNecessary<T, TDictionary> : Necessary<T, MatchProperty[]>
        {
            private readonly PropertyDescriptorsInfo _left;
            private readonly PropertyDescriptorsInfo _right;
            public DictionaryNecessary()
            {
                _left = new PropertyDescriptorsInfo(typeof(TDictionary), PropertyDescriptorKind.Dictionary);
                _right = new PropertyDescriptorsInfo(typeof(T), this);
            }
            protected override MatchProperty[] GetReady(IEnumerable<T> items)
            {
                return JoinMatches(_left, _right, this.Load, items);
            }
            private void Load(IEnumerable<T> items)
            {
                _left.LoadDictionaryProperties();
                _right.LoadProperties(items);
            }
        }
        private static void LoadDictionaryProperties(this PropertyDescriptorsInfo info)
        {
            if (TryGetDictionaryValueType(info.Type, out Type valueType))
            {
                info.DictionaryValueType = valueType;
            }
            info.Properties = new PropertyDescriptor[0];
        }
        private static TDictionary ToDictionary<T, TDictionary>(this T item, MatchProperty[] matches)
            where TDictionary : IDictionary, new()
            where T : class
        {
            TDictionary dictionary = new TDictionary();
            if (item != null)
            {
                foreach (MatchProperty match in matches)
                {
                    match.Apply(dictionary, item);
                }
            }
            return dictionary;
        }
        public static IEnumerable<TDictionary> ToDictionaries<T, TDictionary>(this IEnumerable<T> items)
            where TDictionary : IDictionary, new()
            where T : class
        {
            if (items != null)
            {
                DictionaryNecessary<T, TDictionary> necessary = new DictionaryNecessary<T, TDictionary>();
                foreach (T item in necessary.Each(items))
                {
                    yield return item.ToDictionary<T, TDictionary>(necessary.Value);
                }
            }
        }
        public static IEnumerable<Dictionary<string, object>> ToDictionaries<T>(this IEnumerable<T> items)
            where T : class
        {
            return ToDictionaries<T, Dictionary<string, object>>(items);
        }
        public static IEnumerable<Dictionary<string, object>> ToDictionaries(this DataTable table)
        {
            return ToDictionaries<Dictionary<string, object>>(table);
        }
        public static IEnumerable<TDictionary> ToDictionaries<TDictionary>(this DataTable table)
            where TDictionary : IDictionary, new()
        {
            return ToDictionaries<DataRow, TDictionary>(table?.AsEnumerable());
        }
    }
}
