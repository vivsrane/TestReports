using System.Collections.Generic;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    /// <summary>
    /// Collection of reports under a common title.
    /// </summary>
    public interface IReportGroup : IReportTreeNode
    {
        /// <summary>
        /// Children of the node when the NodeType is 'Node' otherwise null
        /// </summary>
        IList<IReportTreeNode> Children
        {
            get;
        }
    }
}
