using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections;

namespace System.ComponentModel
{
    internal class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        private readonly string _key;
        private readonly Type _type;
        public override Type ComponentType
        {
            get
            {
                return typeof(IDictionary);
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return false;
            }
        }
        public override Type PropertyType
        {
            get
            {
                return _type;
            }
        }
        public override string DisplayName
        {
            get
            {
                return string.Empty;
            }
        }
        public DictionaryPropertyDescriptor(string key, Type type)
            : base(key.ToString(), null)
        {
            _key = key;
            _type = type;
        }
        public override bool CanResetValue(object component)
        {
            return this.GetValue(component) != null;
        }
        public override object GetValue(object component)
        {
            return ((IDictionary)component)[_key];
        }
        public override void ResetValue(object component)
        {
            this.SetValue(component, null);
        }
        public override void SetValue(object component, object value)
        {
            IDictionary dictionary = (IDictionary)component;
            if (!dictionary.Contains(_key) || dictionary[_key] != value)
            {
                dictionary[_key] = value;
                this.OnValueChanged(component, EventArgs.Empty);
            }
        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
