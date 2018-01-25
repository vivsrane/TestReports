
using VB.Reports.App.ReportDefinitionLibrary;

namespace BV.AppCode
{
    public static class IReportListHelper
    {
        public static void PopulateReportList(IReportList list, IReportTreeNode root)
        {
            list.Title = root.Title;

            list.Reports.Clear();

            if (root.NodeType.Equals(ReportTreeNodeType.ReportGroup))
            {
                IReportGroup group = (IReportGroup) root;

                foreach (IReportTreeNode child in group.Children)
                {
                    list.Reports.Add((IReportHandle) child);
                }
            }
        }
    }
}
