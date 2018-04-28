﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class Properties
    {
        private static object Convert(object value, Type targetType, out bool done)
        {
            if (targetType.IsEnum)
            {
                try
                {
                    if (value is string)
                    {
                        value = Enum.Parse(targetType, (string)value);
                    }
                    else
                    {
                        value = Enum.ToObject(targetType, value);
                    }
                    done = true;
                    return value;
                }
                catch
                {
                    done = false;
                    return value;
                }
            }
            else if (value is IConvertible)
            {
                if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    targetType = targetType.GetGenericArguments()[0];
                }
                try
                {
                    value = System.Convert.ChangeType(value, targetType);
                    done = true;
                    return value;
                }
                catch
                {
                    done = false;
                    return value;
                }
            }
            else
            {
                done = true;
                return value;
            }
        }
    }
}