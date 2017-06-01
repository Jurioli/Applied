using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class Properties
    {
        private static Type GetMatchType(Type type)
        {
            if (typeof(IConvertible).IsAssignableFrom(type.GetNullableUnderlyingType()))
            {
                return null;
            }
            return type;
        }
        private static Type GetNullableUnderlyingType(this Type type)
        {
            if (type.IsNullable())
            {
                return Nullable.GetUnderlyingType(type);
            }
            else
            {
                return type;
            }
        }
        internal static bool IsNullable(this Type type)
        {
            if (type != null && type.IsValueType && type.IsGenericType && !type.IsGenericTypeDefinition
                && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return true;
            }
            return false;
        }
    }
}
