using System;
using System.Web;

namespace BV.AppCode
{
    public class SessionStateHttpModule : IHttpModule
    {
        public static readonly string REPORT_CENTER_SESSION_KEY = "ReportCenterSession";
        public static readonly string REPORT_CENTER_SESSION_ID  = "ReportCenterSession.SessionID";

        public SessionStateHttpModule()
        {
            // no state to initialize
        }

        public void Dispose()
        {
            // no state to destroy
        }

        public void Init(HttpApplication application)
        {
            application.PostAcquireRequestState += OnPostAcquireRequestState;
        }

        public void OnPostAcquireRequestState(object sender, EventArgs args)
        {
            /*
            Member member = MemberFinder.Instance().FindByLogin(HttpContext.Current.User.Identity.Name);

            SoftwareSystemComponent component = SoftwareSystemComponentFinder.Instance().FindByToken("DEALER_GROUP_SYSTEM_COMPONENT");

            SoftwareSystemComponentState authorization = SoftwareSystemComponentStateFinder.Instance().FindByAuthorizedMemberAndSoftwareSystemComponent(member, component);

            if (authorization == null)
            {
                switch (member.MemberType)
                {
                    case MemberType.Administrator:
                    case MemberType.AccountRepresentative:
                        // they will go to a business unit selection page so do not create authorization
                        break;
                    case MemberType.User:
                        // gather the dealer group and dealer records
                        IEnumerator<BusinessUnit> enDealerGroup = member.DealerGroups.GetEnumerator();
                        enDealerGroup.MoveNext();
                        IEnumerator<BusinessUnit> enDealer = member.DealerGroups.GetEnumerator();
                        enDealer.MoveNext();
                        // create the authorization record
                        authorization = new SoftwareSystemAuthorization();
                        authorization.SoftwareSystemComponent.SetValue(component);
                        authorization.AuthorizedMember.SetValue(member);
                        authorization.DealerGroup.SetValue(enDealerGroup.Current);
                        authorization.Dealer.SetValue(enDealer.Current);
                        authorization.Save();
                        break;
                    default:
                        // ignore dummy users
                        break;
                }
            }
            else
            {
                switch (member.MemberType)
                {
                    case MemberType.Administrator:
                    case MemberType.AccountRepresentative:
                        // they will go to a business unit selection page so do not create authorization
                        break;
                    default:
                        break;
                }
            }*/

            /*string sessionID = HttpContext.Current.Request.QueryString["SessionID"];

            if (sessionID == null && HttpContext.Current.Session != null)
            {
                sessionID = (string) HttpContext.Current.Session[REPORT_CENTER_SESSION_ID];
            }

            bool haveReportCenterSession = false;

            if (sessionID != null)
            {
                ReportCenterSessionFinder finder = ReportCenterSessionFinder.Instance();

                HttpContext.Current.Session[REPORT_CENTER_SESSION_ID] = sessionID;

                IDictionary<string,object> whereClause = finder.CreateWhereClause("SessionId", sessionID);

                whereClause.Add("Active", true);

                ReportCenterSession reportCenterSession = finder.FindFirst(whereClause);

                if (reportCenterSession != null)
                {
                    HttpContext.Current.Session[REPORT_CENTER_SESSION_KEY] = reportCenterSession;

                    haveReportCenterSession = true;
                }
                else
                {
                    HttpContext.Current.Session.Remove(REPORT_CENTER_SESSION_KEY);
                }
            }

            if (HttpContext.Current.Session != null && !haveReportCenterSession)
            {
                try
                {
                    MemberFinder finder = MemberFinder.Instance();

                    // load the member using the Principal information
                    Member member = finder.FindFirst(finder.CreateWhereClause("Login", HttpContext.Current.User.Identity.Name));

                    // load their active session
                    ReportCenterSession reportCenterSession = null;
                    try
                    {
                        reportCenterSession = ReportCenterSessionFinder.Instance().FindActiveByMemberId(member.Id);
                    }
                    catch
                    {
                        // make a new session for the user
                        switch (member.MemberType)
                        {
                            case MemberType.Administrator:
                                // they will go to a business unit selection page so do not create a session
                                break;
                            case MemberType.AccountRepresentative:
                                // they do not use the reporting application
                                break;
                            case MemberType.Dummy:
                                // this is an incorrectly setup user
                                break;
                            case MemberType.User:
                                // add a new session
                                if (member.DealerGroups.Count == 1)
                                {
                                    reportCenterSession = new ReportCenterSession();
                                    reportCenterSession.SessionId = Guid.NewGuid().ToString();
                                    reportCenterSession.Member.SetValue(member);
                                    reportCenterSession.Active = true;

                                    IEnumerator<BusinessUnit> en = member.Dealers.GetEnumerator();
                                    en.MoveNext();
                                    BusinessUnit dealer = en.Current;

                                    en = member.DealerGroups.GetEnumerator();
                                    en.MoveNext();
                                    BusinessUnit dealerGroup = en.Current;

                                    if (member.Dealers.Count == 1)
                                    {
                                        reportCenterSession.ParentBusinessUnit.SetValue(null);
                                        reportCenterSession.BusinessUnit.SetValue(dealer);
                                    }
                                    else
                                    {
                                        reportCenterSession.ParentBusinessUnit.SetValue(dealerGroup);
                                        reportCenterSession.BusinessUnit.SetValue(dealer);
                                    }
                                    
                                    reportCenterSession.Save();
                                }
                                break;
                        }
                    }

                    if (reportCenterSession != null)
                    {
                        HttpContext.Current.Session[REPORT_CENTER_SESSION_ID] = reportCenterSession.SessionId;
                        HttpContext.Current.Session[REPORT_CENTER_SESSION_KEY] = reportCenterSession;
                    }
                }
                catch (Exception)
                {
                    HttpContext.Current.Session.Remove(REPORT_CENTER_SESSION_ID);
                    HttpContext.Current.Session.Remove(REPORT_CENTER_SESSION_KEY);
                }
            }*/
        }
    }

}