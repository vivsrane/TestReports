using System;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class RemoteReportProcessingLocation : IRemoteReportProcessingLocation
    {
        #region IRemoteReportProcessingLocation Members

        private string path;
        private Uri serverUrl;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public Uri ServerUrl
        {
            get { return serverUrl; }
            set { serverUrl = value; }
        }

        #endregion
    }
}
