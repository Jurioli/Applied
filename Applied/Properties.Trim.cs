using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Data;

namespace System
{
    public static partial class Properties
    {
        private class TrimNecessary : Necessary<PropertyDescriptor[]>
        {
            private readonly PropertyDescriptorsInfo _info;
            public PropertyDescriptorKind Kind
            {
                get
                {
                    return _info.Kind;
                }
            }
            public TrimNecessary(Type type)
            {
                _info = new PropertyDescriptorsInfo(type, this);
            }
            protected override PropertyDescriptor[] GetReady(IEnumerable items)
            {
                _info.LoadProperties(items);
                return GetTrimPropertyDescriptors(_info);
            }
        }
        private static PropertyDescriptor[] GetTrimPropertyDescriptors(PropertyDescriptorsInfo info)
        {
            return GetTrimPropertyDescriptors(info.Properties).ToArray();
        }
        private static IEnumerable<PropertyDescriptor> GetTrimPropertyDescriptors(PropertyDescriptor[] properties)
        {
            foreach (PropertyDescriptor property in properties)
            {
                if (property.PropertyType == typeof(string) && !property.IsReadOnly)
                {
                    yield return property;
                }
            }
        }
        private static void Trim(this object source, PropertyDescriptorKind kind, PropertyDescriptor[] properties, char[] trimChars)
        {
            if (source == null)
            {
                return;
            }
            if (source is DataTable dataTable)
            {
                dataTable.AsEnumerable().Trim(typeof(DataRow), trimChars);
                return;
            }
            string value;
            foreach (PropertyDescriptor property in properties)
            {
                value = (string)property.GetValue(source);
                if (!string.IsNullOrEmpty(value))
                {
                    property.SetValue(source, value.Trim(trimChars));
                }
            }
            if (kind == PropertyDescriptorKind.Class)
            {
                if (source is IEnumerable items)
                {
                    if (TryGetEnumerableType(source.GetType(), out Type elementType))
                    {
                        items.Trim(elementType, trimChars);
                    }
                }
            }
        }
        private static void Trim(this IEnumerable items, Type elementType, char[] trimChars)
        {
            TrimNecessary necessary = new TrimNecessary(elementType);
            foreach (object item in necessary.Each(items))
            {
                item.Trim(necessary.Kind, necessary.Value, trimChars);
            }
        }
        public static TSource Trim<TSource>(this TSource source, params char[] trimChars) where TSource : class
        {
            if (source != null)
            {
                new TSource[] { source }.Trim(typeof(TSource), trimChars);
            }
            return source;
        }
        public static DataTable Trim(this DataTable source, params char[] trimChars)
        {
            if (source != null)
            {
                source.AsEnumerable().Trim(typeof(DataRow), trimChars);
            }
            return source;
        }
    }
}
