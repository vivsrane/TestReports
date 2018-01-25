using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class ReportGroup : ReportTreeNode, IReportGroup
    {
        #region IReportCategory Members

        private readonly IList<IReportTreeNode> children;

        public ReportGroup()
        {
            children = new List<IReportTreeNode>();
        }

        IList<IReportTreeNode> IReportGroup.Children
        {
            get { return new ReadOnlyCollection<IReportTreeNode>(children); }
        }

        public IList<IReportTreeNode> Children
        {
            get { return children; }
        }

        #endregion
    }
}
