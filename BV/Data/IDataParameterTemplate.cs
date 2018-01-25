using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Data
{
    public interface IDataParameterTemplate
    {
        /// <summary>
        /// Name of the parameter
        /// </summary>
        string Name { get;  }


        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db", Justification = "Framework Design Guideline: Consider naming properties after their type.")]
        System.Data.DbType DbType { get; }

        /// <summary>
        /// Whether null is a valid parameter value
        /// </summary>
        bool IsNullable { get; }
    }
}
