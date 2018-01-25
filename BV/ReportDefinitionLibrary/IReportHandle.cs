namespace VB.Reports.App.ReportDefinitionLibrary
{
    /// <summary>
    /// Reference to, or a placeholder for, a report.
    /// </summary>
    public interface IReportHandle : IReportTreeNode
    {
        /// <summary>
        /// The report
        /// </summary>
        IReport Report
        {
            get;
        }

        /// <summary>
        /// Indicates whether the report is present or not
        /// </summary>
        bool ComingSoon
        {
            get;
        }
    }
}
