using System;

namespace VB.Common.Core.Validation.Constraints
{
    public class BetweenValueConstraint<T> : IConstraint<T> where T : IComparable<T>
    {
        private readonly string _resourceKey;

        private readonly T _min, _max;

        public BetweenValueConstraint(T min, T max)
            : this("global.constraint.value.between", min, max)
        {
        }

        public BetweenValueConstraint(string resourceKey, T min, T max)
        {
            _min = min;
            _max = max;
            _resourceKey = resourceKey;
        }

        public T Min
        {
            get { return _min; }
        }

        public T Max
        {
            get { return _max; }
        }

        public bool IsSatisfiedBy(T value)
        {
// ReSharper disable CompareNonConstrainedGenericWithNull
            if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
            {
                return true;
            }

            if (value.CompareTo(Min) == -1 || value.CompareTo(Max) == 1)
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
