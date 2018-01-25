using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VB.DomainModel.Oltp.WebControls
{
    /// <summary>
    /// Paramter that resolves to the current user to a Member entity.
    /// </summary>
    [Serializable]
    public class MemberParameter : Parameter
    {
        private bool allowImpersonation;

        public bool AllowImpersonation
        {
            get { return allowImpersonation; }
            set { allowImpersonation = value; }
        }

        public MemberParameter()
            : base()
        {
            allowImpersonation = true;
        }

        protected MemberParameter(MemberParameter original)
            : base(original)
        {
            this.allowImpersonation = original.allowImpersonation;
        }

        protected override object Evaluate(HttpContext context, Control control)
        {
            SoftwareSystemComponentState state = (SoftwareSystemComponentState) context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];

            //Member member = null;

            if (allowImpersonation)
            {
                //member = state.DealerGroupMember();
            }
            else
            {
               // member = state.AuthorizedMember.GetValue();
            }

            return null;
        }

        protected override Parameter Clone()
        {
            return new MemberParameter(this);
        }
    }
}
