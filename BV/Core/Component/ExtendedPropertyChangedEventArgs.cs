using System;
using System.ComponentModel;

namespace VB.Common.Core.Component
{
    [Serializable]
    public class ExtendedPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        private readonly object _oldValue;
        private readonly object _newValue;

        public ExtendedPropertyChangedEventArgs(string propertyName, object oldValue, object newValue)
            : base(propertyName)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public object OldValue
        {
            get { return _oldValue; }
        }

        public object NewValue
        {
            get { return _newValue; }
        }
    }
}