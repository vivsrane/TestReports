using System.Collections.Generic;
using System.Linq;

namespace VB.Common.Core.Validation.Constraints
{
    public class CompositeConstraint<T> : IConstraint<T> where T : class, IConstrained
    {
        private readonly string _resourceKey;

        public CompositeConstraint(): this("global.constraint.composite.item")
        {
        }

        public CompositeConstraint(string resourceKey)
        {
            _resourceKey = resourceKey;
        }

        public bool IsSatisfiedBy(T value)
        {
            if (value == null)
            {
                return true;
            }

            if (!value.IsValid)
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

    public class CompositeCollectionConstraint<T> : IConstraint<IList<T>> where T : IConstrained
    {
        private readonly string _resourceKey;

        public CompositeCollectionConstraint(): this("global.constraint.composite.collection")
        {
        }

        public CompositeCollectionConstraint(string resourceKey)
        {
            _resourceKey = resourceKey;
        }

        public bool IsSatisfiedBy(IList<T> value)
        {
            if (value == null || value.Count == 0)
            {
                return true;
            }

            if (value.Any(x => !x.IsValid))
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
