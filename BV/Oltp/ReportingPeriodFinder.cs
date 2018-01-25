using VB.Common.ActiveRecord;

namespace VB.DomainModel.Oltp
{
    public class ReportingPeriodFinder : Finder<ReportingPeriod>
    {
        private static readonly ReportingPeriodFinder finder = new ReportingPeriodFinder();

        /*A private constructor would make sense here since this is following the singleton pattern but
         objects creating this through reflection need a public default constructor*/
        public ReportingPeriodFinder()
        {
        }

        public static ReportingPeriodFinder Instance()
        {
            return finder;
        }
    }
}
