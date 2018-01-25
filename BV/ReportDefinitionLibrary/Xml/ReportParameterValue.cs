namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class ReportParameterValue : IReportParameterValue
    {
        #region IReportParameterValue Members

        private string value;
        private string label;
        private bool selected;

        public string Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public string Label
        {
            get
            {
                return label;
            }
            set
            {
                label = value;
            }
        }

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
            }
        }

        #endregion
    }
}
