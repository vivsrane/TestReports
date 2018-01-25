namespace VB.Common.Core.Validation.Constraints
{
    public class RequiredConstraint<T> : IConstraint<T> where T : class
    {
        private readonly string _resourceKey;

        public RequiredConstraint()
            : this("global.constraint.required")
        {
        }

        public RequiredConstraint(string resourceKey)
        {
            _resourceKey = resourceKey;
        }

        public bool IsSatisfiedBy(T value)
        {
            return (value != null);
        }

        public string ResourceKey
        {
            get { return _resourceKey; }
        }

        public bool StopProcessing
        {
            get { return true; }
        }
    }
}