using VB.Common.Core.Component;

namespace VB.Common.Core.Validation.Constraints
{
    public class PropertyConstraint<TType, TProperty> : IConstraint<TType>
    {
        private readonly PropertyDelegate<TType, TProperty> _getter;

        private readonly IConstraint<TProperty> _constraint;

        public PropertyConstraint(PropertyDelegate<TType, TProperty> getter, IConstraint<TProperty> constraint)
        {
            _getter = getter;
            _constraint = constraint;
        }

        public bool IsSatisfiedBy(TType value)
        {
            return _constraint.IsSatisfiedBy(_getter(value));
        }

        public string ResourceKey
        {
            get { return _constraint.ResourceKey; }
        }

        public bool StopProcessing
        {
            get { return _constraint.StopProcessing; }
        }
    }
}