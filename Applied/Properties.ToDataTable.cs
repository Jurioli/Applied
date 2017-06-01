using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Applied;

namespace System
{
    public static partial class Properties
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                return null;
            }
            DataTable table = new DataTable();
            T temp = default(T);
            Lazy<MatchProperty[]> matches = new Lazy<MatchProperty[]>(() =>
            {
                Type type = typeof(T);
                return (from a in GetProperties(type, type.IsNeedGetter() ? new Lazy(() => temp) : null)
                        where TryAddColumn(table, a)
                        select new MatchProperty()
                        {
                            Right = a,
                            Left = new DataRowPropertyDescriptor(table.Columns[a.Name])
                        }).ToArray();
            });
            DataRow row = null;
            foreach (T item in items)
            {
                temp = item;
                if (matches.IsValueCreated)
                {
                    row = table.NewRow();
                    foreach (MatchProperty match in matches.Value)
                    {
                        match.Left.SetValue(row, match.Right.GetValue(item));
                    }
                }
                else
                {
                    MatchProperty[] m = matches.Value;
                    row = table.NewRow();
                    foreach (MatchProperty match in m)
                    {
                        match.Left.SetValue(row, match.Right.GetValue(item));
                    }
                }
                table.Rows.Add(row);
            }
            return table;
        }
        private static bool TryAddColumn(DataTable table, PropertyDescriptor property)
        {
            try
            {
                table.Columns.Add(property.Name, property.PropertyType.GetNullableUnderlyingType());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
