using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace System.ComponentModel
{
    internal class DataRowPropertyDescriptor : PropertyDescriptor
    {
        private readonly DataColumn _column;
        public override Type ComponentType
        {
            get
            {
                return typeof(DataRow);
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return _column.ReadOnly;
            }
        }
        public override Type PropertyType
        {
            get
            {
                return _column.DataType;
            }
        }
        public override string DisplayName
        {
            get
            {
                return _column.Caption;
            }
        }
        public override AttributeCollection Attributes
        {
            get
            {
                if (typeof(IList).IsAssignableFrom(this.PropertyType))
                {
                    List<Attribute> list = new List<Attribute>(base.Attributes.Cast<Attribute>());
                    list.Add(new ListBindableAttribute(false));
                    return new AttributeCollection(list.ToArray());
                }
                return base.Attributes;
            }
        }
        public override bool IsBrowsable
        {
            get
            {
                if (_column.ColumnMapping == MappingType.Hidden)
                {
                    return false;
                }
                return base.IsBrowsable;
            }
        }
        public DataRowPropertyDescriptor(DataColumn column)
            : base(FormatPropertyName(column.ColumnName), null)
        {
            _column = column;
        }
        private static string FormatPropertyName(string columnName)
        {
            return Regex.Replace(columnName, "[\\W]{1}", m =>
            {
                return "_";
            });
        }
        public override bool CanResetValue(object component)
        {
            return this.GetValue(component) != null;
        }
        public override object GetValue(object component)
        {
            object value = ((DataRow)component)[_column];
            if (value == DBNull.Value)
            {
                value = null;
            }
            return value;
        }
        public override void ResetValue(object component)
        {
            this.SetValue(component, null);
        }
        public override void SetValue(object component, object value)
        {
            if (this.GetValue(component) != value)
            {
                if (value == null)
                {
                    value = DBNull.Value;
                }
                ((DataRow)component)[_column] = value;
                this.OnValueChanged(component, EventArgs.Empty);
            }
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
