using System;
using VB.Common.Core.Utilities;
using VB.DomainModel.Oltp;

namespace BV.Controls
{
    public partial class Controls_Reports_SelectDealerReportsView : System.Web.UI.UserControl
    {
        protected void DealershipReportSelect_DataBound(object sender, EventArgs e)
        {
            SoftwareSystemComponentState state = (SoftwareSystemComponentState) Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
           
        }

        private bool SelectDealershipListItem(string value)
        {
            for (int i = 0; i < DealershipReportSelect.Items.Count; i++)
            {
                if (DealershipReportSelect.Items[i].Value.Equals(value))
                {
                    DealershipReportSelect.SelectedIndex = i;
                    return true;
                }
            }
            return false;
        }

        protected void DealershipReportSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!DealershipReportSelect.SelectedValue.Equals("0"))
            {
                SoftwareSystemComponentState state = (SoftwareSystemComponentState)Context.Items[SoftwareSystemComponentStateFacade.HttpContextKey];
              
                state.Save();
            }
        }

    }
}
