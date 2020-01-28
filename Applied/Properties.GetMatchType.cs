using System;
using System.Applied;
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
                return typeof(IConvertible);
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
        private static string GetFullName(this Type type)
        {
            return type.FullName + ";" + type.Assembly.FullName;
        }
        private static bool TryGetEnumerableType(this Type type, out Type elementType)
        {
            if (type.IsArray)
            {
                elementType = type.GetElementType();
                return true;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                elementType = type.GetGenericArguments()[0];
                return true;
            }
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    elementType = interfaceType.GetGenericArguments()[0];
                    return true;
                }
            }
            elementType = null;
            return false;
        }
        private static object GetTypedNull(Type type)
        {
            return ComponentOperator.GetTypedDefaultMethod(type)();
        }
    }
}
