namespace VB.Common.Core.Validation.Constraints
{
    public class ExactLengthConstraint : IConstraint<string>
    {
        private readonly string _resourceKey;

        private readonly int _length;

        public ExactLengthConstraint(int length)
            : this("global.constraint.length.exact", length)
        {
        }

        public ExactLengthConstraint(string resourceKey, int length)
        {
            _length = length;
            _resourceKey = resourceKey;
        }

        public int Length
        {
            get { return _length; }
        }

        public bool IsSatisfiedBy(string value)
        {
            if (value == null)
            {
                return true;
            }

            if (value.Length != Length)
            {
                return false;
            }

            return true;
        }

        public string ResourceKey
        {
            get { return _resourceKey; }
        }

        public bool StopProcessing
        {
            get { return false; }
        }
    }
}
