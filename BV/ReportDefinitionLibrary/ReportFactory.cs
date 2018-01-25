namespace VB.Reports.App.ReportDefinitionLibrary
{
    /// <remarks>Abstract factory pattern with static factory method</remarks>
    public abstract class ReportFactory
    {
        public abstract IReport FindReport(string id);

        public abstract IReport FindReportByServerPath(string path);

        public abstract bool IsAuthorized(string id, int businessUnitId);

        internal abstract void CompleteReport(Xml.Report report);

        public static ReportFactory NewReportFactory(string path)
        {
            return new Xml.XmlReportFactory(path);
        }

    }
}
