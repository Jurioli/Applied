using System.ComponentModel;
using System.Data;

namespace System
{
    public static partial class Properties
    {
        private abstract class MatchProperty
        {
            protected readonly PropertyDescriptor _left;
            protected readonly PropertyDescriptor _right;
            public MatchProperty(PropertyDescriptor left, PropertyDescriptor right)
            {
                _left = left;
                _right = right;
            }
            public abstract void Apply(object source, object newValue);
            public void ApplyDataRow<T>(DataRow row, T item)
            {
                _left.SetValue(row, _right.GetValue(item));
            }
        }
        private class EnumMatchProperty : MatchProperty
        {
            private readonly Type _targetType;
            public EnumMatchProperty(PropertyDescriptor left, PropertyDescriptor right, Type targetType)
                : base(left, right)
            {
                _targetType = targetType;
            }
            public override void Apply(object source, object newValue)
            {
                object value = _right.GetValue(newValue);
                try
                {
                    if (value is string str)
                    {
                        value = Enum.Parse(_targetType, str);
                    }
                    else
                    {
                        value = Enum.ToObject(_targetType, value);
                    }
                }
                catch
                {
                    value = GetTypedNull(_left.PropertyType);
                }
                _left.SetValue(source, value);
            }
        }
        private class ConvertibleMatchProperty : MatchProperty
        {
            private readonly Type _targetType;
            public ConvertibleMatchProperty(PropertyDescriptor left, PropertyDescriptor right, Type targetType)
                : base(left, right)
            {
                _targetType = targetType;
            }
            public override void Apply(object source, object newValue)
            {
                object value = _right.GetValue(newValue);
                if (value is IConvertible)
                {
                    try
                    {
                        value = Convert.ChangeType(value, _targetType);
                    }
                    catch
                    {
                        value = GetTypedNull(_left.PropertyType);
                    }
                }
                else
                {
                    value = GetTypedNull(_left.PropertyType);
                }
                _left.SetValue(source, value);
            }
        }
        private class ValueTypeMatchProperty : MatchProperty
        {
            public ValueTypeMatchProperty(PropertyDescriptor left, PropertyDescriptor right)
                : base(left, right)
            {

            }
            public override void Apply(object source, object newValue)
            {
                object value = _right.GetValue(newValue);
                if (value == null)
                {
                    value = GetTypedNull(_left.PropertyType);
                }
                _left.SetValue(source, value);
            }
        }
        private class DefaultMatchProperty : MatchProperty
        {
            public DefaultMatchProperty(PropertyDescriptor left, PropertyDescriptor right)
                : base(left, right)
            {

            }
            public override void Apply(object source, object newValue)
            {
                object value = _right.GetValue(newValue);
                _left.SetValue(source, value);
            }
        }
        private static MatchProperty GetMatchProperty(PropertyDescriptor left, PropertyDescriptor right, bool valueType)
        {
            Type leftType = left.PropertyType;
            Type rightType = right.PropertyType;
            if (!leftType.IsAssignableFrom(rightType))
            {
                if (leftType.IsValueType && leftType.IsGenericType && !leftType.IsGenericTypeDefinition
                    && leftType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    leftType = leftType.GetGenericArguments()[0];
                }
                if (leftType.IsEnum)
                {
                    return new EnumMatchProperty(left, right, leftType);
                }
                else
                {
                    return new ConvertibleMatchProperty(left, right, leftType);
                }
            }
            if (valueType && leftType.IsValueType)
            {
                return new ValueTypeMatchProperty(left, right);
            }
            return new DefaultMatchProperty(left, right);
        }
    }
}
