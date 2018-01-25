using VB.Common.ActiveRecord;

namespace VB.DomainModel.Oltp
{
    public class SoftwareSystemComponentFinder : Finder<SoftwareSystemComponent>
    {
        private static readonly SoftwareSystemComponentFinder finder = new SoftwareSystemComponentFinder();

        private SoftwareSystemComponentFinder()
        {
        }

        public static SoftwareSystemComponentFinder Instance()
        {
            return finder;
        }

        public SoftwareSystemComponent FindByToken(string token)
        {
            return FindFirst(CreateWhereClause("Token", token));
        }
    }
}
