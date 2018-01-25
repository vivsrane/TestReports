﻿namespace VB.Common.Core.Validation.Constraints
{
    public class MaxLengthConstraint : IConstraint<string>
    {
        private readonly string _resourceKey;

        private readonly int _length;

        public MaxLengthConstraint(int length)
            : this("global.constraint.length.max", length)
        {
        }

        public MaxLengthConstraint(string resourceKey, int length)
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

            if (value.Length > Length)
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