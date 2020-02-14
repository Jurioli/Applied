using System.Applied;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;

namespace System
{
    public static partial class Properties
    {
        private class TrimNecessary : NecessaryFirst<PropertyDescriptor[]>
        {
            private PropertyDescriptorsInfo _info;
            public PropertyDescriptorKind Kind
            {
                get
                {
                    return _info.Kind;
                }
            }
            protected override void First(object first)
            {
                _info = new PropertyDescriptorsInfo(first.GetType(), this);
            }
            protected override PropertyDescriptor[] GetReady(IEnumerable items)
            {
                _info.LoadProperties(items);
                return GetTrimPropertyDescriptors(_info.Properties).ToArray();
            }
        }
        private class TrimDataRowNecessary
        {
            private readonly DataTable _table;
            private readonly Lazy<PropertyDescriptor[]> _lazy;
            public PropertyDescriptor[] Value
            {
                get
                {
                    return _lazy.Value;
                }
            }
            public TrimDataRowNecessary(DataTable table)
            {
                _table = table;
                _lazy = new Lazy<PropertyDescriptor[]>(GetReady);
            }
            private PropertyDescriptor[] GetReady()
            {
                PropertyDescriptor[] properties = GetPropertyDescriptors(_table);
                return GetTrimPropertyDescriptors(properties).ToArray();
            }
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
                dataTable.TrimDataTable(trimChars);
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
                    items.TrimEnumerable(trimChars);
                }
            }
        }
        private static void TrimEnumerable(this IEnumerable items, char[] trimChars)
        {
            TrimNecessary necessary = new TrimNecessary();
            foreach (object item in necessary.Each(items))
            {
                item.Trim(necessary.Kind, necessary.Value, trimChars);
            }
        }
        public static TSource Trim<TSource>(this TSource source, params char[] trimChars) where TSource : class
        {
            if (source != null)
            {
                Type type = source.GetType();
                if (type.IsArray)
                {
                    ((IEnumerable)source).TrimEnumerable(trimChars);
                }
                else
                {
                    PropertyDescriptorsInfo info = new PropertyDescriptorsInfo(type);
                    info.LoadProperties(new TSource[] { source });
                    PropertyDescriptor[] properties = GetTrimPropertyDescriptors(info.Properties).ToArray();
                    source.Trim(info.Kind, properties, trimChars);
                }
            }
            return source;
        }
        private static void TrimDataRow(this DataRow row, PropertyDescriptor[] properties, char[] trimChars)
        {
            string value;
            foreach (PropertyDescriptor property in properties)
            {
                value = (string)property.GetValue(row);
                if (!string.IsNullOrEmpty(value))
                {
                    property.SetValue(row, value.Trim(trimChars));
                }
            }
        }
        private static void TrimDataTable(this DataTable table, char[] trimChars)
        {
            TrimDataRowNecessary necessary = new TrimDataRowNecessary(table);
            foreach (DataRow row in table.AsEnumerable())
            {
                row.TrimDataRow(necessary.Value, trimChars);
            }
        }
        public static DataTable Trim(this DataTable source, params char[] trimChars)
        {
            if (source != null)
            {
                source.TrimDataTable(trimChars);
            }
            return source;
        }
    }
}
