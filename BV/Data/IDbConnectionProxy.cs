using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Data
{
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Db", Justification = "Consistency with System.Data namespace")]
    public interface IDbConnectionProxy
    {
        IDbConnection Connection();
    }
}
