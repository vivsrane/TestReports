namespace VB.Common.Core.Validation.Constraints
{
    public class BetweenLengthConstraint : IConstraint<string>
    {
        private readonly string _resourceKey;

        private readonly int _min, _max;

        public BetweenLengthConstraint(int min, int max)
            : this("global.constraint.length.between", min, max)
        {
        }

        public BetweenLengthConstraint(string resourceKey, int min, int max)
        {
            _min = min;
            _max = max;
            _resourceKey = resourceKey;
        }

        public int Min
        {
            get { return _min; }
        }

        public int Max
        {
            get { return _max; }
        }

        public bool IsSatisfiedBy(string value)
        {
            if (value == null)
            {
                return true;
            }

            int length = value.Length;

            if (length < Min || length > Max)
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
