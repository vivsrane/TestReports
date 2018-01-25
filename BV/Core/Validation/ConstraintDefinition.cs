using System.Collections.Generic;

namespace VB.Common.Core.Validation
{
    public abstract class ConstraintDefinition<T> : IConstraintDefinition<T> where T : class
    {
        private readonly IList<IConstraint<T>> _objectConstraints;

        private readonly IDictionary<string, IList<IConstraint<T>>> _propertyConstraints;

        protected ConstraintDefinition()
        {
            _objectConstraints = new List<IConstraint<T>>();

            _propertyConstraints = new Dictionary<string, IList<IConstraint<T>>>();
        }

        public IList<IConstraint<T>> ObjectConstraints
        {
            get { return _objectConstraints; }
        }

        public IDictionary<string, IList<IConstraint<T>>> PropertyConstraints
        {
            get { return _propertyConstraints; }
        }
    }
}