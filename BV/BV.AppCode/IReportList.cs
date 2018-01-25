using System.Collections.Generic;
using VB.Reports.App.ReportDefinitionLibrary;

namespace BV.AppCode
{
    public interface IReportList
    {
        string Title { get; set; }

        IList<IReportHandle> Reports { get; }
    }
}