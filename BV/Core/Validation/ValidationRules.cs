using System.Collections.Generic;

namespace VB.Common.Core.Validation
{
    public interface IValidationRules
    {
        IConstraintViolations ConstraintViolations { get; }

        bool IsValid { get; }

        void CheckRules();

        void CheckRules(string propertyName);

        void CheckRules(string propertyName, object oldValue, object newValue);
    }

    public class ValidationRules<T> : IValidationRules
    {
        private readonly T _value;

        private readonly IConstraintDefinition<T> _definition;

        private IConstraintViolations _violations;

        public ValidationRules(T value, IConstraintDefinition<T> definition)
        {
            _value = value;

            _definition = definition;
        }

        public IConstraintViolations ConstraintViolations
        {
            get
            {
                if (_violations == null)
                {
                    _violations = new ConstraintViolations();
                }

                return _violations;
            }
        }

        public void CheckRules()
        {
            foreach (KeyValuePair<string, IList<IConstraint<T>>> pair in _definition.PropertyConstraints)
            {
                CheckRules(pair.Key, pair.Value, null, null);
            }

            foreach (IConstraint<T> constraint in _definition.ObjectConstraints)
            {
                var violation = new ObjectConstraintViolation(typeof(T).Name, constraint.ResourceKey);

                if (constraint.IsSatisfiedBy(_value))
                {
                    ConstraintViolations.Remove(violation);
                }
                else
                {
                    ConstraintViolations.Add(violation);

                    if (constraint.StopProcessing)
                    {
                        break;
                    }
                }
            }
        }

        public void CheckRules(string propertyName)
        {
            CheckRules(propertyName, null, null);
        }

        public void CheckRules(string propertyName, object oldValue, object newValue)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                CheckRules();
            }
            else
            {
                IList<IConstraint<T>> constraints = _definition.PropertyConstraints[propertyName];

                if (constraints != null)
                {
                    CheckRules(propertyName, constraints, oldValue, newValue);
                }
            }
        }

        private void CheckRules(string propertyName, IEnumerable<IConstraint<T>> constraints, object oldValue, object newValue)
        {
            foreach (IConstraint<T> constraint in constraints)
            {
                var violation = new PropertyConstraintViolation(
                        typeof(T).Name,
                        constraint.ResourceKey,
                        propertyName,
                        oldValue,
                        newValue);

                if (constraint.IsSatisfiedBy(_value))
                {
                    ConstraintViolations.Remove(violation);
                }
                else
                {
                    ConstraintViolations.Add(violation);

                    if (constraint.StopProcessing)
                    {
                        break;
                    }
                }
            }
        }

        public bool IsValid
        {
            get { return ConstraintViolations.Count == 0; }
        }
    }
}