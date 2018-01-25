using System;

namespace VB.DomainModel.Oltp
{
    /// <summary>
    /// Summary description for SoftwareSystemComponentStateFacade
    /// </summary>
    public static class SoftwareSystemComponentStateFacade
    {
        public static readonly string DealerGroupComponentToken = "DEALER_GROUP_SYSTEM_COMPONENT";

        public static readonly string DealerComponentToken = "DEALER_SYSTEM_COMPONENT";

        public static readonly string HttpContextKey = "SoftwareSystemComponentState";

        public static SoftwareSystemComponentState FindOrCreate(string userName, string token, int dealerId)
        {
            throw new NotImplementedException();
            //Member member = MemberFinder.Instance().FindByUserName(userName);

            //if (member.MemberType == MemberType.Administrator)
            //{
            //    BusinessUnit dealer = BusinessUnitFinder.Instance().Find(dealerId);

            //    return FindOrCreate(userName, token, member, dealer.DealerGroup(), dealer);
            //}

            //foreach (BusinessUnit dealer in member.Dealers)
            //{
            //    if (dealer.Id == dealerId)
            //    {
            //        return FindOrCreate(userName, token, member, dealer.DealerGroup(), dealer);
            //    }
            //} 

            //throw new ArgumentException("No access to Dealer", "dealerId");
        }

        //public static SoftwareSystemComponentState FindOrCreate(string userName, string token)
        //{
        //    //Member member = MemberFinder.Instance().FindByUserName(userName);

        //    //return FindOrCreate(userName, token, member, dealerGroup, dealer);
        //    throw new NotImplementedException();
        //}

        //private static SoftwareSystemComponentState FindOrCreate(string userName, string token)
        //{
            //SoftwareSystemComponentState state = SoftwareSystemComponentStateFinder.Instance().FindByAuthorizedMemberAndSoftwareSystemComponent(userName, token);
            //if (AllowAccessToSoftwareSystemComponent(token, member))
            //{
            //    if (member.AccessibleDealers(dealerGroup).Contains(dealer))
            //    {
            //        if (state == null)
            //        {
            //            state = new SoftwareSystemComponentState();
            //        }
            //        state.SoftwareSystemComponent.SetValue(GetComponent(token));
            //        //state.AuthorizedMember.SetValue(member);
            //        //state.DealerGroup.SetValue(dealerGroup);
            //        //state.Dealer.SetValue(dealer);
            //        //state.Member.SetValue(null);
            //        state.Save();
            //    }
            //}

            //return state;
        //    throw new NotImplementedException();
        //}

        public static SoftwareSystemComponentState FindOrCreate(string userName, string token)
        {
            SoftwareSystemComponentState state = SoftwareSystemComponentStateFinder.Instance().FindByAuthorizedMemberAndSoftwareSystemComponent(userName, token);

            if (state == null)
            {
                //Member member = MemberFinder.Instance().FindByUserName(userName);

                //if (member.MemberType.Equals(MemberType.User))
                //{
                //    if (AllowAccessToSoftwareSystemComponent(token, member))
                //    {
                //        BusinessUnit dealer = CollectionHelper.First(member.Dealers);

                //        state = new SoftwareSystemComponentState();
                //        state.SoftwareSystemComponent.SetValue(GetComponent(token));
                //        state.AuthorizedMember.SetValue(member);
                //        state.DealerGroup.SetValue(dealer.DealerGroup());
                //        state.Dealer.SetValue(dealer);
                //        state.Member.SetValue(null);
                //        state.Save();
                //    }
                //}
            }

            return state;
        }

        private static SoftwareSystemComponent GetComponent(string token)
        {
            return SoftwareSystemComponentFinder.Instance().FindByToken(token);
        }

        //private static bool AllowAccessToSoftwareSystemComponent(string token, Member member)
        //{
        //    bool allow = false;
        //    switch (member.Dealers.Count)
        //    {
        //        case 0:
        //            allow |= member.MemberType == MemberType.Administrator;
        //            allow |= member.MemberType == MemberType.AccountRepresentative;
        //            break;
        //        case 1:
        //            allow = DealerComponentToken.Equals(token);
        //            break;
        //        default:
        //            allow |= DealerGroupComponentToken.Equals(token);
        //            allow |= DealerComponentToken.Equals(token);
        //            break;
        //    }
        //    return allow;
        //}
    }
}