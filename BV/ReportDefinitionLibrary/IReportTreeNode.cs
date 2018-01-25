namespace VB.Reports.App.ReportDefinitionLibrary
{
    public interface IReportTreeNode
    {
        string Title
        {
            get;
        }

        /// <summary>
        /// Description of the node
        /// </summary>
        string Description
        {
            get;
        }

        /// <summary>
        /// Represents the type of the underlying object
        /// </summary>
        ReportTreeNodeType NodeType
        {
            get;
        }
    }
}
