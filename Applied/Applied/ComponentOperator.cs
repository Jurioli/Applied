using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

namespace System.Applied
{
    internal delegate void Setter(object obj, object value);
    internal delegate object Getter(object obj);
    internal static class ComponentOperator
    {
        private delegate PropertyInfo PropertyGetter(PropertyDescriptor property);
        private static readonly PropertyGetter propertyGetter;
        static ComponentOperator()
        {
            try
            {
                Type reflectPropertyType = typeof(PropertyDescriptor).Assembly.GetType("System.ComponentModel.ReflectPropertyDescriptor");
                ParameterExpression ParameterExpression1 = Expression.Parameter(typeof(PropertyDescriptor), "obj");
                ConditionalExpression ConditionalExpression1 = Expression.Condition(Expression.TypeIs(ParameterExpression1, reflectPropertyType),
                    Expression.Field(Expression.Convert(ParameterExpression1, reflectPropertyType), "propInfo"),
                    Expression.Constant(null, typeof(PropertyInfo)));
                Expression<PropertyGetter> LambdaExpression1 = Expression.Lambda<PropertyGetter>(ConditionalExpression1, ParameterExpression1);
                propertyGetter = LambdaExpression1.Compile();
            }
            catch
            {
                propertyGetter = new PropertyGetter(p => null);
            }
        }
        public static PropertyInfo GetPropertyInfo(PropertyDescriptor property)
        {
            return propertyGetter(property);
        }
        public static Setter GetPropertySetter(Type type, Type valueType, PropertyInfo propertyInfo)
        {
            ParameterExpression ParameterExpression1 = Expression.Parameter(typeof(object), "obj");
            ParameterExpression ParameterExpression2 = Expression.Parameter(typeof(object), "value");
            Expression instanceExpression;
            if (!type.IsAssignableFrom(typeof(object)))
            {
                instanceExpression = Expression.Convert(ParameterExpression1, type);
            }
            else
            {
                instanceExpression = ParameterExpression1;
            }
            Expression argumentExpression;
            if (!valueType.IsAssignableFrom(typeof(object)))
            {
                argumentExpression = Expression.Convert(ParameterExpression2, valueType);
            }
            else
            {
                argumentExpression = ParameterExpression2;
            }
            Expression MemberExpression1 = Expression.Property(instanceExpression, propertyInfo);
            if (!MemberExpression1.Type.IsNullable())
            {
                Expression BinaryExpression1 = ExpressionExtensions.Assign(MemberExpression1, argumentExpression);
                Expression<Setter> LambdaExpression1 = Expression.Lambda<Setter>(BinaryExpression1, ParameterExpression1, ParameterExpression2);
                return LambdaExpression1.Compile();
            }
            else
            {
                MethodInfo methodInfo = propertyInfo.GetSetMethod();
                MethodCallExpression MethodCallExpression1 = Expression.Call(instanceExpression, methodInfo, argumentExpression);
                Expression<Setter> LambdaExpression1 = Expression.Lambda<Setter>(MethodCallExpression1, ParameterExpression1, ParameterExpression2);
                return LambdaExpression1.Compile();
            }
        }
        public static Getter GetPropertyGetter(Type type, PropertyInfo propertyInfo)
        {
            ParameterExpression ParameterExpression1 = Expression.Parameter(typeof(object), "obj");
            MemberExpression MemberExpression1;
            if (!type.IsAssignableFrom(typeof(object)))
            {
                MemberExpression1 = Expression.Property(Expression.Convert(ParameterExpression1, type), propertyInfo);
            }
            else
            {
                MemberExpression1 = Expression.Property(ParameterExpression1, propertyInfo);
            }
            if (!MemberExpression1.Type.IsAssignableFrom(typeof(object)))
            {
                UnaryExpression UnaryExpression1 = Expression.Convert(MemberExpression1, typeof(object));
                Expression<Getter> LambdaExpression1 = Expression.Lambda<Getter>(UnaryExpression1, ParameterExpression1);
                return LambdaExpression1.Compile();
            }
            else
            {
                Expression<Getter> LambdaExpression1 = Expression.Lambda<Getter>(MemberExpression1, ParameterExpression1);
                return LambdaExpression1.Compile();
            }
        }
        public static Setter GetPropertySetter(Type type, Type valueType, string propertyName)
        {
            ParameterExpression ParameterExpression1 = Expression.Parameter(typeof(object), "obj");
            ParameterExpression ParameterExpression2 = Expression.Parameter(typeof(object), "value");
            Expression instanceExpression;
            if (!type.IsAssignableFrom(typeof(object)))
            {
                instanceExpression = Expression.Convert(ParameterExpression1, type);
            }
            else
            {
                instanceExpression = ParameterExpression1;
            }
            Expression argumentExpression;
            if (!valueType.IsAssignableFrom(typeof(object)))
            {
                argumentExpression = Expression.Convert(ParameterExpression2, valueType);
            }
            else
            {
                argumentExpression = ParameterExpression2;
            }
            Expression MemberExpression1 = Expression.Property(instanceExpression, propertyName);
            if (!MemberExpression1.Type.IsNullable())
            {
                Expression BinaryExpression1 = ExpressionExtensions.Assign(MemberExpression1, argumentExpression);
                Expression<Setter> LambdaExpression1 = Expression.Lambda<Setter>(BinaryExpression1, ParameterExpression1, ParameterExpression2);
                return LambdaExpression1.Compile();
            }
            else
            {
                MethodInfo methodInfo = instanceExpression.Type.GetProperty(propertyName).GetSetMethod();
                MethodCallExpression MethodCallExpression1 = Expression.Call(instanceExpression, methodInfo, argumentExpression);
                Expression<Setter> LambdaExpression1 = Expression.Lambda<Setter>(MethodCallExpression1, ParameterExpression1, ParameterExpression2);
                return LambdaExpression1.Compile();
            }
        }
        public static Getter GetPropertyGetter(Type type, string propertyName)
        {
            ParameterExpression ParameterExpression1 = Expression.Parameter(typeof(object), "obj");
            MemberExpression MemberExpression1;
            if (!type.IsAssignableFrom(typeof(object)))
            {
                MemberExpression1 = Expression.Property(Expression.Convert(ParameterExpression1, type), propertyName);
            }
            else
            {
                MemberExpression1 = Expression.Property(ParameterExpression1, propertyName);
            }
            if (!MemberExpression1.Type.IsAssignableFrom(typeof(object)))
            {
                UnaryExpression UnaryExpression1 = Expression.Convert(MemberExpression1, typeof(object));
                Expression<Getter> LambdaExpression1 = Expression.Lambda<Getter>(UnaryExpression1, ParameterExpression1);
                return LambdaExpression1.Compile();
            }
            else
            {
                Expression<Getter> LambdaExpression1 = Expression.Lambda<Getter>(MemberExpression1, ParameterExpression1);
                return LambdaExpression1.Compile();
            }
        }
    }
    internal static class ExpressionExtensions
    {
        public static BinaryExpression Assign(Expression left, Expression right)
        {
            MethodInfo assign = typeof(Assigner<>).MakeGenericType(left.Type).GetMethod("Assign");
            return Expression.Equal(left, right, false, assign);
        }
        private static class Assigner<T>
        {
            public static T Assign(ref T left, T right)
            {
                return left = right;
            }
        }
    }
}