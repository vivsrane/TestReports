using System;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class XmlReportGroupingFactory : ReportGroupingFactory
    {
        private readonly string path;
        private readonly ReportFactory reportFactory;

        protected internal XmlReportGroupingFactory(string path, ReportFactory reportFactory)
        {
            this.path = path;
            this.reportFactory = reportFactory;
        }

        public override IReportTreeNode BuildReportTree(ReportType reportType, int businessUnitId)
        {
            Serialization.ReportGroupingMetadata metadata = (Serialization.ReportGroupingMetadata)Serialization.ReportDefinitionLibraryMetadataCache.GetReportDefinitionLibraryMetadata(path, typeof(Serialization.ReportGroupingMetadata));

            Serialization.ReportGroupingType reportGroupingType;

            if (reportType == ReportType.Dealer)
            {
                reportGroupingType = Serialization.ReportGroupingType.Dealer;
            }
            else
            {
                reportGroupingType = Serialization.ReportGroupingType.DealerGroup;
            }

            IReportTreeNode root = null;

            foreach (Serialization.ReportGrouping reportGrouping in metadata.ReportGroupings)
            {
                if (!reportGrouping.ReportGroupingType.Equals(reportGroupingType))
                    continue;

                root = Copy(reportGrouping.Node, businessUnitId);

                break;
            }

            return root;
        }

        public override IReportTreeNode BuildReportTree(string id, int businessUnitId)
        {
            Serialization.ReportGroupingMetadata metadata = (Serialization.ReportGroupingMetadata)Serialization.ReportDefinitionLibraryMetadataCache.GetReportDefinitionLibraryMetadata(path, typeof(Serialization.ReportGroupingMetadata));

            IReportTreeNode root = null;

            foreach (Serialization.ReportGrouping reportGrouping in metadata.ReportGroupings)
            {
                Serialization.Node node = FindNode(reportGrouping.Node, id);

                if (node != null)
                {
                    root = Copy(node, businessUnitId);
                    break;
                }
            }

            return root;
        }

        private static Serialization.Node FindNode(Serialization.Node node, string id)
        {
            if (node.Id.Equals(id))
            {
                return node;
            }
            else if (node.NodeType.Equals(Serialization.NodeType.ReportHandle))
            {
                return null;
            }
            else
            {
                foreach (Serialization.Node child in node.Content.Items)
                {
                    Serialization.Node result = FindNode(child, id);

                    if (result != null)
                    {
                        return result;
                    }
                }

                return null;
            }
        }

        private ReportTreeNode Copy(Serialization.Node node, int businessUnitId)
        {
            if (node.NodeType.Equals(Serialization.NodeType.ReportGroup))
            {
                return CopyGroup(node, businessUnitId);
            }
            else
            {
                return CopyReport(node, businessUnitId);
            }
        }

        private ReportTreeNode CopyGroup(Serialization.Node src, int businessUnitId)
        {
            ReportGroup dst = new ReportGroup();
            
            CopyFields(dst, src);

            foreach (Serialization.Node node in src.Content.Items)
            {
                ReportTreeNode child = Copy(node, businessUnitId);

                if (child != null)
                {
                    dst.Children.Add(child);
                }
            }

            return dst;
        }

        private ReportTreeNode CopyReport(Serialization.Node src, int businessUnitId)
        {
            ReportTreeNode dst = null;
            if (reportFactory.IsAuthorized(((Serialization.ReportReference)src.Content.Items[0]).Value, businessUnitId))
            {
                ReportHandle handle = new ReportHandle();

                CopyFields(handle, src);

                Serialization.ReportReference reference = (Serialization.ReportReference) src.Content.Items[0];

                if (reference.ComingSoon)
                {
                    handle.ComingSoon = true;
                }
                else
                {
                    handle.Report = reportFactory.FindReport(reference.Value);
                }

                dst = handle;
            }

            return dst;
        }

        private static void CopyFields(ReportTreeNode dst, Serialization.Node src)
        {
            dst.Title = src.Title;
            dst.Description = src.Description;
            dst.NodeType = (ReportTreeNodeType) Enum.Parse(typeof(ReportTreeNodeType), src.NodeType.ToString());
        }
    }
}
