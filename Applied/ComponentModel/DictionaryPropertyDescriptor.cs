using System.Collections;

namespace System.ComponentModel
{
    internal class DictionaryPropertyDescriptor : PropertyDescriptor
    {
        private readonly string _key;
        private readonly Type _type;
        private readonly bool _valuesNull;
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
        internal bool ValuesNull
        {
            get
            {
                return _valuesNull;
            }
        }
        public DictionaryPropertyDescriptor(string key, Type type)
            : this(key, type, false)
        {

        }
        public DictionaryPropertyDescriptor(string key, Type type, bool valuesNull)
            : base(key, null)
        {
            _key = key;
            _type = type;
            _valuesNull = valuesNull;
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
