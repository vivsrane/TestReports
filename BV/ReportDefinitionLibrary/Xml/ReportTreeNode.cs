namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal abstract class ReportTreeNode : IReportTreeNode
    {
        #region IReportTreeNode Members

        private string title;
        private string description;
        private ReportTreeNodeType nodeType;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public ReportTreeNodeType NodeType
        {
            get { return nodeType; }
            set { nodeType = value; }
        }

        #endregion
    }
}
