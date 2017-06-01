using System;
using System.Collections.Generic;
using System.Linq;

namespace System.Applied
{
    [Serializable]
    internal class Lazy
    {
        private readonly object _padlock = new object();
        private readonly Func<object> _valueFactory;
        private bool _isValueCreated;
        private object value;
        public object Value
        {
            get
            {
                if (!_isValueCreated)
                {
                    lock (_padlock)
                    {
                        if (!_isValueCreated)
                        {
                            value = _valueFactory();
                            _isValueCreated = true;
                        }
                    }
                }
                return value;
            }
        }
        public bool IsValueCreated
        {
            get
            {
                lock (_padlock)
                {
                    return _isValueCreated;
                }
            }
        }
        public Lazy(Func<object> valueFactory)
        {
            this._valueFactory = valueFactory ?? throw new ArgumentNullException("valueFactory");
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
    [Serializable]
    internal class Lazy<T>
    {
        private readonly object _padlock = new object();
        private readonly Func<T> _valueFactory;
        private bool _isValueCreated;
        private T value;
        public T Value
        {
            get
            {
                if (!_isValueCreated)
                {
                    lock (_padlock)
                    {
                        if (!_isValueCreated)
                        {
                            value = _valueFactory();
                            _isValueCreated = true;
                        }
                    }
                }
                return value;
            }
        }
        public bool IsValueCreated
        {
            get
            {
                lock (_padlock)
                {
                    return _isValueCreated;
                }
            }
        }
        public Lazy(Func<T> valueFactory)
        {
            this._valueFactory = valueFactory ?? throw new ArgumentNullException("valueFactory");
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}