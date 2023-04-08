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
        private static string GetPropertyName(PropertyDescriptor property)
        {
            ApplyPropertyNameAttribute attribute = property.Attributes[typeof(ApplyPropertyNameAttribute)] as ApplyPropertyNameAttribute;
            return attribute != null ? attribute.Name : property.Name;
        }
        public ComponentPropertyDescriptor(PropertyDescriptor property)
            : base(GetPropertyName(property), null)
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
                _getter = new Lazy<Getter>(() => ComponentOperator.GetPropertyGetter(_componentType, property.Name));
                _setter = new Lazy<Setter>(() => ComponentOperator.GetPropertySetter(_componentType, _propertyType, property.Name));
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
