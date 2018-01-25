
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.ActiveRecord
{
    /// <summary>
    /// Provide access to a single valued association value.
    /// </summary>
    /// <typeparam name="TTargetEntity">The type that is the target of the association.</typeparam>
    public interface ISingleValuedAssociation<TTargetEntity> where TTargetEntity : class
    {
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        TTargetEntity GetValue();

        void SetValue(TTargetEntity value);
    }
}
