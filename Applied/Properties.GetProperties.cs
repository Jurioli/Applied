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
        private static readonly Dictionary<string, PropertyDescriptor[]> _typeDescriptorDictionary = new Dictionary<string, PropertyDescriptor[]>();
        private static PropertyDescriptor[] GetProperties(Type componentType, Lazy componentGetter)
        {
            string key;
            return GetProperties(componentType, componentGetter, out key).Value;
        }
        private static Lazy<PropertyDescriptor[]> GetProperties(Type componentType, Lazy componentGetter, out string key)
        {
            if (componentType == typeof(DataRow))
            {
                key = null;
                return new Lazy<PropertyDescriptor[]>(() => (from a in ((DataRow)componentGetter.Value).Table.Columns.Cast<DataColumn>()
                                                             select new DataRowPropertyDescriptor(a)).ToArray());
            }
            if (typeof(IDictionary).IsAssignableFrom(componentType))
            {
                try
                {
                    IDictionary dictionary = (IDictionary)componentGetter.Value;
                    string[] keys = dictionary.Keys.Cast<string>().ToArray();
                    key = null;
                    return new Lazy<PropertyDescriptor[]>(() => (from a in keys
                                                                 select new DictionaryPropertyDescriptor(a, dictionary[a] != null ? dictionary[a].GetType() : typeof(object))).ToArray());
                }
                catch (InvalidCastException) { }
            }
            string fullName = componentType.FullName + ";" + componentType.Assembly.FullName;
            key = fullName;
            return new Lazy<PropertyDescriptor[]>(() =>
            {
                PropertyDescriptor[] result;
                if (!_typeDescriptorDictionary.TryGetValue(fullName, out result))
                {
                    lock (_typeDescriptorDictionary)
                    {
                        if (!_typeDescriptorDictionary.TryGetValue(fullName, out result))
                        {
                            result = TypeDescriptor.GetProperties(componentType).Cast<PropertyDescriptor>().Select(a => new ComponentPropertyDescriptor(a)).ToArray();
                            _typeDescriptorDictionary.Add(fullName, result);
                        }
                    }
                }
                return result;
            });
        }
        private static bool IsNeedGetter(this Type componentType)
        {
            return componentType == typeof(DataRow) || typeof(IDictionary).IsAssignableFrom(componentType);
        }
        public static PropertyDescriptorCollection GetProperties(this DataTable table)
        {
            return new PropertyDescriptorCollection((from a in table.Columns.Cast<DataColumn>()
                                                     select new DataRowPropertyDescriptor(a)).ToArray());
        }
        public static PropertyDescriptorCollection GetProperties<TDictionary>(this IEnumerable<TDictionary> dictionaries)
            where TDictionary : IDictionary
        {
            if (dictionaries.Any())
            {
                try
                {
                    dictionaries = dictionaries.Take(16).ToArray();
                    string[] keys = dictionaries.SelectMany(d => d.Keys.Cast<string>()).Distinct().ToArray();
                    Dictionary<string, Type> keysDictionary = keys.ToDictionary(a => a, a =>
                    {
                        TDictionary dictionary = dictionaries.Where(d => d.Contains(a) && d[a] != null).FirstOrDefault();
                        if (dictionary != null)
                        {
                            return dictionary[a].GetType();
                        }
                        return typeof(object);
                    });
                    return new PropertyDescriptorCollection((from a in keysDictionary
                                                             select new DictionaryPropertyDescriptor(a.Key, a.Value)).ToArray());
                }
                catch (InvalidCastException) { }
            }
            return PropertyDescriptorCollection.Empty;
        }
    }
}