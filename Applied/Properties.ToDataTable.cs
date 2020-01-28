using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Data;

namespace System
{
    public static partial class Properties
    {
        private class DataRowNecessary<T> : MatchesNecessary<T>
        {
            private readonly DataTable _table;
            private readonly PropertyDescriptorsInfo _right;
            private readonly PropertyDescriptorsInfo _left;
            public DataRowNecessary(DataTable table)
            {
                _table = table;
                _right = new PropertyDescriptorsInfo(typeof(T), this);
                _left = new PropertyDescriptorsInfo(typeof(DataRow), PropertyDescriptorKind.DataRow);
            }
            public override void LoadProperties(IEnumerable<T> items)
            {
                _right.LoadProperties(items);
                _left.LoadDataRowProperties(_table, _right.Properties);
            }
            protected override MatchProperty[] GetReady(IEnumerable<T> items)
            {
                return JoinMatches(_left, _right, this, items);
            }
        }
        private static void LoadDataRowProperties(this PropertyDescriptorsInfo info, DataTable table, PropertyDescriptor[] properties)
        {
            info.Properties = GetDataRowPropertyDescriptors(table, properties).ToArray();
        }
        private static IEnumerable<DataRowPropertyDescriptor> GetDataRowPropertyDescriptors(DataTable table, PropertyDescriptor[] properties)
        {
            foreach (PropertyDescriptor a in properties)
            {
                DataColumn column;
                try
                {
                    column = table.Columns.Add(a.Name, a.PropertyType.GetNullableUnderlyingType());
                }
                catch { continue; }
                yield return new DataRowPropertyDescriptor(column);
            }
        }
        private static void AddDataRow<T>(this DataTable table, T item, MatchProperty[] matches)
        {
            DataRow row = table.NewRow();
            if (item != null)
            {
                foreach (MatchProperty match in matches)
                {
                    match.Left.SetValue(row, match.Right.GetValue(item));
                }
            }
            table.Rows.Add(row);
        }
        public static DataTable ToDataTable<T>(this IEnumerable<T> items)
        {
            DataTable table = new DataTable();
            if (items != null)
            {
                DataRowNecessary<T> necessary = new DataRowNecessary<T>(table);
                foreach (T item in necessary.Each(items))
                {
                    table.AddDataRow(item, necessary.Value);
                }
            }
            return table;
        }
    }
}
