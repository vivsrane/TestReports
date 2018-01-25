namespace VB.Common.Core.Validation
{
    public class ObjectConstraintViolation : IConstraintViolation
    {
        private readonly string _objectName;
        private readonly string _resourceKey;

        public ObjectConstraintViolation(string objectName, string resourceKey)
        {
            _objectName = objectName;
            _resourceKey = resourceKey;
        }

        public virtual object[] Arguments
        {
            get { return new object[] { _objectName }; }
        }

        public string ObjectName
        {
            get { return _objectName; }
        }

        public string ResourceKey
        {
            get { return _resourceKey; }
        }

        public bool Equals(ObjectConstraintViolation obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._objectName, _objectName) && Equals(obj._resourceKey, _resourceKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ObjectConstraintViolation)) return false;
            return Equals((ObjectConstraintViolation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_objectName != null ? _objectName.GetHashCode() : 0)*397) ^
                       (_resourceKey != null ? _resourceKey.GetHashCode() : 0);
            }
        }
    }
}