using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VB.DomainModel.Oltp.WebControls
{
    /// <summary>
    /// Parameter representing the current users dealer group.
    /// </summary>
    [Serializable]
    public class DealerParameter : Parameter
    {
        public DealerParameter()
            : base()
        {
        }

        protected DealerParameter(DealerParameter original)
            : base(original)
        {
        }

        protected override object Evaluate(HttpContext context, Control control)
        {
            SoftwareSystemComponentState state = (SoftwareSystemComponentState)context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];

            return null; //state.Dealer.GetValue();
        }

        protected override Parameter Clone()
        {
            return new DealerParameter(this);
        }
    }
}
