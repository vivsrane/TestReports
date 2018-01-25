using System.Collections.Generic;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;
using VB.Common.ActiveRecord;

namespace VB.DomainModel.Oltp
{
    public class SoftwareSystemComponentStateFinder : Finder<SoftwareSystemComponentState>
    {
        private static readonly SoftwareSystemComponentStateFinder finder = new SoftwareSystemComponentStateFinder();

        private SoftwareSystemComponentStateFinder()
        {
        }

        public static SoftwareSystemComponentStateFinder Instance()
        {
            return finder;
        }

        public SoftwareSystemComponentState FindByAuthorizedMemberAndSoftwareSystemComponent(Member member, SoftwareSystemComponent softwareSystemComponent)
        {
            IDictionary<string, object> bindings = CreateWhereClause("authorizedMemberId", 0);
            bindings.Add("softwareSystemComponentId", softwareSystemComponent.Id);
            return FindFirst(bindings);
        }

        public SoftwareSystemComponentState FindByAuthorizedMemberAndSoftwareSystemComponent(string userName, string softwareSystemComponentToken)
        {
            string commandText = @"
                SELECT
            " + GetRowDataGateway().ColumnList("S") + @"
                FROM
		                dbo.SoftwareSystemComponentState S
                JOIN	dbo.SoftwareSystemComponent C ON C.SoftwareSystemComponentID = S.SoftwareSystemComponentID
                JOIN    dbo.Member M ON M.MemberID = S.AuthorizedMemberID
                WHERE
	                    C.Token = @SoftwareSystemComponentToken
                AND	    M.Login = @Login";

            IList<RowDataGatewayBinding> parameterList = CreateBindingList("Login", userName, DbType.String, false);
            parameterList.Add(new RowDataGatewayBinding("SoftwareSystemComponentToken", softwareSystemComponentToken, DbType.String, false));
            return First(Find(commandText, parameterList));
        }
    }
}
