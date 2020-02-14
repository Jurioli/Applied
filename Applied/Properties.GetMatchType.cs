using System.Applied;

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
        private static int GetDeterministicHashCode(this string value)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;
                for (int i = 0; i < value.Length; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ value[i];
                    if (i == value.Length - 1)
                    {
                        break;
                    }
                    hash2 = ((hash2 << 5) + hash2) ^ value[i + 1];
                }
                return hash1 + (hash2 * 1566083941);
            }
        }
        private static string GetFullName(this Type type)
        {
            return type.FullName + ";" + type.Assembly.FullName;
        }
        internal static int GetFullNameHashCode(this Type type)
        {
            return type.GetFullName().GetDeterministicHashCode();
        }
        private static int GetFullNameHashCode(Type leftType, Type rightType)
        {
            return (leftType.GetFullName() + "#" + rightType.GetFullName()).GetDeterministicHashCode();
        }
        private static object GetTypedNull(Type type)
        {
            return ComponentOperator.GetTypedDefaultMethod(type)();
        }
    }
}
