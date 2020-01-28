using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace System
{
    public static partial class Properties
    {
        private enum PropertyDescriptorKind
        {
            Class,
            DataRow,
            Dictionary
        }
        private class PropertyDescriptorsInfo
        {
            public Type Type { get; private set; }
            public PropertyDescriptorKind Kind { get; private set; }
            public PropertyDescriptor[] Properties { get; set; }
            public Type DictionaryValueType { get; set; }
            public PropertyDescriptorsInfo(Type type, PropertyDescriptorKind kind)
            {
                this.Type = type;
                this.Kind = kind;
            }
            public PropertyDescriptorsInfo(Type type, INecessary necessary) : this(type, GetKind(type))
            {
                if (this.Kind == PropertyDescriptorKind.DataRow)
                {
                    necessary.UpdateCount(1);
                }
                else if (this.Kind == PropertyDescriptorKind.Dictionary)
                {
                    necessary.UpdateCount(16);
                }
            }
            private static PropertyDescriptorKind GetKind(Type type)
            {
                if (type == typeof(DataRow))
                {
                    return PropertyDescriptorKind.DataRow;
                }
                else if (typeof(IDictionary).IsAssignableFrom(type))
                {
                    return PropertyDescriptorKind.Dictionary;
                }
                else
                {
                    return PropertyDescriptorKind.Class;
                }
            }
            public void ConcatDictionaryProperties(PropertyDescriptor[] properties)
            {
                properties = this.GetConcatDictionaryProperties(properties).ToArray();
                PropertyDescriptor[] concatProperties = new PropertyDescriptor[this.Properties.Length + properties.Length];
                for (int i = 0; i < concatProperties.Length; i++)
                {
                    if (i < this.Properties.Length)
                    {
                        concatProperties[i] = this.Properties[i];
                    }
                    else
                    {
                        concatProperties[i] = properties[i - this.Properties.Length];
                    }
                }
                this.Properties = concatProperties;
            }
            private IEnumerable<PropertyDescriptor> GetConcatDictionaryProperties(PropertyDescriptor[] properties)
            {
                List<string> names = this.Properties.Select(a => a.Name).ToList();
                if (this.DictionaryValueType != null)
                {
                    foreach (PropertyDescriptor property in properties)
                    {
                        if (names.Contains(property.Name) == false)
                        {
                            yield return new DictionaryPropertyDescriptor(property.Name, this.DictionaryValueType);
                        }
                    }
                }
                else
                {
                    foreach (PropertyDescriptor property in properties)
                    {
                        if (names.Contains(property.Name) == false)
                        {
                            yield return new DictionaryPropertyDescriptor(property.Name, property.PropertyType);
                        }
                    }
                }
            }
            public void LoadProperties(IEnumerable items)
            {
                if (this.Kind == PropertyDescriptorKind.DataRow)
                {
                    this.Properties = GetPropertyDescriptors(items.Cast<DataRow>().First());
                }
                else if (this.Kind == PropertyDescriptorKind.Dictionary)
                {
                    try
                    {
                        IDictionary[] dictionaries = items.Cast<IDictionary>().ToArray();
                        string[] keys = dictionaries.SelectMany(a => a.Keys.Cast<string>()).Distinct().ToArray();
                        if (TryGetDictionaryValueType(this.Type, out Type valueType))
                        {
                            this.Properties = GetPropertyDescriptors(keys, valueType);
                            this.DictionaryValueType = valueType;
                        }
                        else
                        {
                            this.Properties = GetPropertyDescriptors(keys, dictionaries);
                        }
                    }
                    catch (InvalidCastException)
                    {
                        this.Kind = PropertyDescriptorKind.Class;
                        this.Properties = GetPropertyDescriptors(this.Type);
                    }
                }
                else
                {
                    this.Properties = GetPropertyDescriptors(this.Type);
                }
            }
        }
    }
}
