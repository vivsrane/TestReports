using System.Collections.Generic;

namespace VB.Common.Core.Validation
{
    public interface IConstraintDefinition<T>
    {
        IList<IConstraint<T>> ObjectConstraints { get; }

        IDictionary<string, IList<IConstraint<T>>> PropertyConstraints { get; }
    }
}