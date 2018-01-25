using System.Collections.Generic;

namespace VB.Common.Core.Validation
{
    public interface IConstraintViolations : IEnumerable<IConstraintViolation>
    {
        void Add(IConstraintViolation violation);

        void Remove(IConstraintViolation violation);

        int Count { get; }

        string Message { get; }
    }
}