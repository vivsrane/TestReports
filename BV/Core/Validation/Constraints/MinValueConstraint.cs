using System;

namespace VB.Common.Core.Validation.Constraints
{
    public class MinValueConstraint<T> : IConstraint<T> where T : IComparable<T>
    {
        private readonly string _resourceKey;

        private readonly T _min;

        public MinValueConstraint(T min)
            : this("global.constraint.value.min", min)
        {
        }

        public MinValueConstraint(string resourceKey, T min)
        {
            _min = min;
            _resourceKey = resourceKey;
        }

        public T Min
        {
            get { return _min; }
        }

        public bool IsSatisfiedBy(T value)
        {
// ReSharper disable CompareNonConstrainedGenericWithNull
            if (value == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
            {
                return true;
            }

            if (value.CompareTo(_min) == -1)
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
