using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace VB.DomainModel.Oltp.WebControls
{
    /// <summary>
    /// Parameter that evaluates to the current users dealer selection in a Dealer Group context.
    /// </summary>
    [Serializable]
    public class DealerSelectionParameter : Parameter
    {
        //public DealerSelectionParameter()
        //{
        //    MemberBusinessUnitSetType = MemberBusinessUnitSetType.CommandCenterDealerGroupSelection;
        //}

        //protected DealerSelectionParameter(DealerSelectionParameter original)
        //    : base(original)
        //{
        //    MemberBusinessUnitSetType = original.MemberBusinessUnitSetType;
        //}

        //protected override object Evaluate(HttpContext context, Control control)
        //{
        //    return context.Items[MemberBusinessUnitSetFacade.HttpContextKey] as ICollection<BusinessUnit>;
        //}

        //protected override Parameter Clone()
        //{
        //    return new DealerSelectionParameter(this);
        //}

        //public MemberBusinessUnitSetType MemberBusinessUnitSetType
        //{
        //    get
        //    {
        //        return (MemberBusinessUnitSetType)ViewState["MemberBusinessUnitSetType"];
        //    }
        //    set
        //    {
        //        ViewState["MemberBusinessUnitSetType"] = value;
        //        OnParameterChanged();
        //    }
        //}
    }
}
