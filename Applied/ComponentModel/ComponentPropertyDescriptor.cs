using System.Applied;
using System.Reflection;

namespace System.ComponentModel
{
    internal class ComponentPropertyDescriptor : PropertyDescriptor
    {
        private readonly Type _componentType;
        private readonly bool _isReadOnly;
        private readonly Type _propertyType;
        private readonly Lazy<Getter> _getter;
        private readonly Lazy<Setter> _setter;
        public override Type ComponentType
        {
            get
            {
                return _componentType;
            }
        }
        public override bool IsReadOnly
        {
            get
            {
                return _isReadOnly;
            }
        }
        public override Type PropertyType
        {
            get
            {
                return _propertyType;
            }
        }
        public ComponentPropertyDescriptor(PropertyDescriptor property)
            : base(property.Name, null)
        {
            _componentType = property.ComponentType;
            _isReadOnly = property.IsReadOnly;
            _propertyType = property.PropertyType;
            PropertyInfo propertyInfo = ComponentOperator.GetPropertyInfo(property);
            if (propertyInfo != null)
            {
                _getter = new Lazy<Getter>(() => ComponentOperator.GetPropertyGetter(_componentType, propertyInfo));
                _setter = new Lazy<Setter>(() => ComponentOperator.GetPropertySetter(_componentType, _propertyType, propertyInfo));
            }
            else
            {
                _getter = new Lazy<Getter>(() => ComponentOperator.GetPropertyGetter(_componentType, this.Name));
                _setter = new Lazy<Setter>(() => ComponentOperator.GetPropertySetter(_componentType, _propertyType, this.Name));
            }
        }
        public override object GetValue(object component)
        {
            return _getter.Value(component);
        }
        public override void SetValue(object component, object value)
        {
            _setter.Value(component, value);
        }
        public override bool CanResetValue(object component)
        {
            return false;
        }
        public override void ResetValue(object component)
        {

        }
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
