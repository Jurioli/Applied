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
        private static readonly Dictionary<int, PropertyDescriptor[]> _typeDescriptorDictionary = new Dictionary<int, PropertyDescriptor[]>();
        public static PropertyDescriptorCollection GetProperties(this DataTable table)
        {
            return new PropertyDescriptorCollection(GetPropertyDescriptors(table));
        }
        public static PropertyDescriptorCollection GetProperties<TDictionary>(this IEnumerable<TDictionary> dictionaries)
            where TDictionary : IDictionary
        {
            if (dictionaries.Any())
            {
                try
                {
                    TDictionary[] items = dictionaries.Take(16).ToArray();
                    string[] keys = items.SelectMany(d => d.Keys.Cast<string>()).Distinct().ToArray();
                    if (TryGetDictionaryValueType(typeof(TDictionary), out Type valueType))
                    {
                        return new PropertyDescriptorCollection(GetPropertyDescriptors(keys, valueType));
                    }
                    else
                    {
                        return new PropertyDescriptorCollection(GetPropertyDescriptors(keys, items));
                    }
                }
                catch (InvalidCastException) { }
            }
            return PropertyDescriptorCollection.Empty;
        }
        private static PropertyDescriptor[] GetPropertyDescriptors(DataRow dataRow)
        {
            return GetPropertyDescriptors(dataRow.Table);
        }
        private static PropertyDescriptor[] GetPropertyDescriptors(DataTable dataTable)
        {
            return GetDataRowPropertyDescriptors(dataTable).ToArray();
        }
        private static IEnumerable<DataRowPropertyDescriptor> GetDataRowPropertyDescriptors(DataTable dataTable)
        {
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                yield return new DataRowPropertyDescriptor(dataColumn);
            }
        }
        private static PropertyDescriptor[] GetPropertyDescriptors(string[] keys, Type valueType)
        {
            return GetDictionaryPropertyDescriptors(keys, valueType).ToArray();
        }
        private static IEnumerable<DictionaryPropertyDescriptor> GetDictionaryPropertyDescriptors(string[] keys, Type valueType)
        {
            foreach (string key in keys)
            {
                yield return new DictionaryPropertyDescriptor(key, valueType);
            }
        }
        private static PropertyDescriptor[] GetPropertyDescriptors<TDictionary>(string[] keys, TDictionary[] dictionaries)
            where TDictionary : IDictionary
        {
            return GetDictionaryPropertyDescriptors(keys, dictionaries).ToArray();
        }
        private static IEnumerable<DictionaryPropertyDescriptor> GetDictionaryPropertyDescriptors<TDictionary>(string[] keys, TDictionary[] dictionaries)
            where TDictionary : IDictionary
        {
            foreach (string key in keys)
            {
                Type valueType = GetDictionaryPropertyDescriptorValueType(key, dictionaries);
                yield return new DictionaryPropertyDescriptor(key, valueType);
            }
        }
        private static Type GetDictionaryPropertyDescriptorValueType<TDictionary>(string key, TDictionary[] dictionaries)
            where TDictionary : IDictionary
        {
            foreach (TDictionary dictionary in dictionaries)
            {
                if (dictionary.Contains(key))
                {
                    object value = dictionary[key];
                    if (value != null)
                    {
                        return value.GetType();
                    }
                }
            }
            return typeof(object);
        }
        private static PropertyDescriptor[] GetPropertyDescriptors(Type componentType)
        {
            int hashCode = componentType.GetFullNameHashCode();
            if (!_typeDescriptorDictionary.TryGetValue(hashCode, out PropertyDescriptor[] result))
            {
                lock (_typeDescriptorDictionary)
                {
                    if (!_typeDescriptorDictionary.TryGetValue(hashCode, out result))
                    {
                        result = GetEntityPropertyDescriptors(componentType).ToArray();
                        _typeDescriptorDictionary.Add(hashCode, result);
                    }
                }
            }
            return result;
        }
        private static IEnumerable<ComponentPropertyDescriptor> GetEntityPropertyDescriptors(Type componentType)
        {
            foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(componentType))
            {
                yield return new ComponentPropertyDescriptor(property);
            }
        }
        private static bool TryGetDictionaryValueType(Type dictionaryType, out Type valueType)
        {
            foreach (Type type in dictionaryType.GetInterfaces())
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                {
                    valueType = type.GetGenericArguments()[1];
                    return valueType != typeof(object);
                }
            }
            valueType = null;
            return false;
        }
    }
}