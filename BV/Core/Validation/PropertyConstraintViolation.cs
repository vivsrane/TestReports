namespace VB.Common.Core.Validation
{
    public class PropertyConstraintViolation : ObjectConstraintViolation
    {
        private readonly string _propertyName;
        private readonly object _oldValue;
        private readonly object _newValue;

        public PropertyConstraintViolation(string objectName, string resourceKey, string propertyName, object oldValue, object newValue) : base(objectName, resourceKey)
        {
            _propertyName = propertyName;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public string PropertyName
        {
            get { return _propertyName; }
        }

        public object OldValue
        {
            get { return _oldValue; }
        }

        public object NewValue
        {
            get { return _newValue; }
        }

        public override object[] Arguments
        {
            get { return new [] { ObjectName, PropertyName, OldValue, NewValue }; }
        }

        public bool Equals(PropertyConstraintViolation obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return base.Equals(obj) && Equals(obj._propertyName, _propertyName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as PropertyConstraintViolation);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = base.GetHashCode();
                result = (result*397) ^ (_propertyName != null ? _propertyName.GetHashCode() : 0);
                return result;
            }
        }
    }
}