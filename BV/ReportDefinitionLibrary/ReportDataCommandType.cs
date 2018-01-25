using System.Diagnostics.CodeAnalysis;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public enum ReportDataCommandType
    {
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Oltp")]
        Oltp = 0,

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Olap")]
        Olap = 1,
    }
}
