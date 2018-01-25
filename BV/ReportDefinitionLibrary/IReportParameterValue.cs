namespace VB.Reports.App.ReportDefinitionLibrary
{
    public interface IReportParameterValue
    {
        /// <summary>
        /// The value of the report parameter
        /// </summary>
        string Value
        {
            get;
        }

        /// <summary>
        /// User facing description of the report parameter value
        /// </summary>
        string Label
        {
            get;
        }

        /// <summary>
        /// Whether the input is selected by default
        /// </summary>
        bool Selected
        {
            get;
        }

    }
}
