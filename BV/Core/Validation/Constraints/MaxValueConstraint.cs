using System;

namespace VB.Common.Core.Validation.Constraints
{
    public class MaxValueConstraint<T> : IConstraint<T> where T : IComparable<T>
    {
        private readonly string _resourceKey;

        private readonly T _max;

        public MaxValueConstraint(T max)
            : this("global.constraint.value.max", max)
        {
        }

        public MaxValueConstraint(string resourceKey, T max)
        {
            _max = max;
            _resourceKey = resourceKey;
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

            if (value.CompareTo(_max) == 1)
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
