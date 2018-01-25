using System;
using System.Runtime.Serialization;

namespace VB.Common.Core.Validation
{
    public class ConstraintViolationException : Exception
    {
        private readonly IConstraintViolations _violations;

        public ConstraintViolationException(IConstraintViolations violations)
            : base(violations.Message)
        {
            _violations = violations;
        }

        protected ConstraintViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info != null)
            {
                _violations = (IConstraintViolations) info.GetValue("Violations", typeof (IConstraintViolations));
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Violations", _violations, typeof (IConstraintViolations));
        }

        public IConstraintViolations Violations
        {
            get { return _violations; }
        }
    }
}