using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using VB.Common.Data;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    public class ReportDataCommandTemplate : IReportDataCommandTemplate
    {
        #region IReportDataCommandTemplate Members

        private string name;
        private string commandText;
        private System.Data.CommandType commandType;
        private string dataSourceName;
        private readonly IList<IDataParameterTemplate> parameters;
        private IDataMap dataMap;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string CommandText
        {
            get { return commandText; }
            set { commandText = value; }
        }

        public string DataSourceName
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            get { return dataSourceName; }
            set { dataSourceName = value; }
        }

        public System.Data.CommandType CommandType
        {
            get { return commandType; }
            set { commandType = value; }
        }

        public IList<IDataParameterTemplate> Parameters
        {
            get { return parameters; }
        }

        public IDataMap DataMap
        {
            get { return dataMap; }
            set { dataMap = value; }
        }

        public ReportDataCommandType ReportDataSourceType(ResolveDataSourceType dsResolver )
        {
            if (dsResolver == null)
                throw new ArgumentNullException("dsResolver", "Data source resolver can not be null!");

            return dsResolver.Invoke(dataSourceName);
        }

        #endregion

        public ReportDataCommandTemplate()
        {
            parameters = new List<IDataParameterTemplate>();
        }

        public ReportDataCommandTemplate(string name) : this()
        {
            Name = name;
        }
    }
}
