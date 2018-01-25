namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class ReportHandle : ReportTreeNode, IReportHandle
    {
        #region IReportHandle Members

        IReport report;
        bool comingSoon;

        public bool ComingSoon
        {
            get { return comingSoon; }
            set { comingSoon = value; }
        }

        public IReport Report
        {
            get { return report; }
            set { report = value;  }
        }

        #endregion
    }
}
