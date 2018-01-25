using System;
using System.Collections.Generic;
using System.Data;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class ReportParameter : IReportParameter
    {
        #region IReportParameter Members

        private string name;
        private string label;
        private bool allowBlank;
        private ReportParameterInputType reportParameterInputType;
        private AbstractReportParameterValues reportParameterValues;
        private DbType dbType;
        private bool isNullable;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public DbType DbType
        {
            get { return dbType; }
            set { dbType = value;}
        }

        public bool IsNullable
        {
            get { return isNullable; }
            set { isNullable = value; }
        }

        public string Label
        {
            get { return label; }
            set { label = value; }
        }

        public bool AllowBlank
        {
            get { return allowBlank; }
            set { allowBlank = value; }
        }

        public ReportParameterInputType ReportParameterInputType
        {
            get { return reportParameterInputType; }
            set { reportParameterInputType = value; }
        }

        public AbstractReportParameterValues ReportParameterValues
        {
            set { reportParameterValues = value; }
        }

        public IList<IReportParameterValue> ValidValues(ResolveParameterValue resolver)
        {
            if (reportParameterValues == null)
                throw new InvalidOperationException("You must set ReportParameterValues before you can retrieve values!");

            return reportParameterValues.GetReportParameterValues(resolver);
        }

        public IReportParameterValue DefaultValue(ResolveParameterValue resolver)
        {
            if (reportParameterValues == null)
                throw new ArgumentNullException("resolver", "You must set ReportParameterValues before you can retrieve values!");

            IList<IReportParameterValue> allValidValues = reportParameterValues.GetReportParameterValues(resolver);
            if (allValidValues == null)
                throw new InvalidOperationException("You must set ValidValues before the DefaultValue can be determined");

            if (allValidValues.Count == 0)
                return null;

            foreach (IReportParameterValue validValue in allValidValues)
            {
                if (validValue.Selected)
                {
                    return validValue;
                }
            }
            return allValidValues[0];
        }

        #endregion
    }
}
