using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Applied;

namespace System
{
    public static partial class Properties
    {
        public static TSource Trim<TSource>(this TSource source, params char[] trimChars) where TSource : class
        {
            if (source != null)
            {
                PropertyDescriptor[] properties = null;
                source.Trim(ref properties, trimChars);
            }
            return source;
        }
        private static PropertyDescriptor[] GetTrimProperties(Type type, Lazy getter)
        {
            return GetProperties(type, getter).Where(a => a.PropertyType == typeof(string) && !a.IsReadOnly).ToArray();
        }
        private static void Trim<TSource>(this TSource source, ref PropertyDescriptor[] properties, params char[] trimChars) where TSource : class
        {
            if (source == null)
            {
                return;
            }
            if (source is DataTable)
            {
                PropertyDescriptor[] rowProperties = null;
                foreach (DataRow row in (source as DataTable).Rows)
                {
                    row.Trim(ref rowProperties, trimChars);
                }
            }
            else
            {
                if (properties == null)
                {
                    Type type = source.GetType();
                    properties = GetTrimProperties(type, type.IsNeedGetter() ? new Lazy(() => source) : null);
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
                if (source is IEnumerable)
                {
                    PropertyDescriptor[] itemProperties = null;
                    foreach (object item in (IEnumerable)source)
                    {
                        item.Trim(ref itemProperties, trimChars);
                    }
                }
            }
        }
    }
}
