using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using ReportViewer = Microsoft.Reporting.WebForms.ReportViewer;

namespace BV.Controls
{
    public partial class ReportNavigation : System.Web.UI.UserControl
    {
        public string ReportViewerID { get; set; }
        public string AfterControlID { get; set; }
        public string CurrentPageControlID { get; set; }
        public string DirtyPageID { get; set; }

        private PagingState _pagingState;

        protected void Page_Load(object sender, EventArgs e)
        {
            _pagingState = new PagingState(this);

            if (!string.IsNullOrEmpty(AfterControlID))
            {
                Control c = Parent.FindControl(AfterControlID);

                if (c != null)
                {
                    c.PreRender += ReportNavigation_PreRender;
                }
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            ReportNavigation_PreRender(sender, e);
        }

        protected void FirstPage_Click(object sender, EventArgs e)
        {
            _pagingState.CurrentPage = 1;
        }

        protected void PreviousPage_Click(object sender, EventArgs e)
        {
            if (_pagingState.CurrentPage > 1)
                _pagingState.CurrentPage = _pagingState.CurrentPage - 1;
        }

        protected void NextPage_Click(object sender, EventArgs e)
        {
            if (_pagingState.CurrentPage < _pagingState.TotalPages)
                _pagingState.CurrentPage = _pagingState.CurrentPage + 1;
        }

        protected void PageTextChanged(object sender, EventArgs e)
        {
            int result;
            if(int.TryParse(PageNumberTextBox.Text, out result))
                _pagingState.CurrentPage = result;
        }


        protected void LastPage_Click(object sender, EventArgs e)
        {
            _pagingState.CurrentPage = _pagingState.TotalPages;
        }
    
   
        private string GetPagingScript(Control pageTextBox, Control pageCount)
        {
            PostBackOptions options = new PostBackOptions(pageTextBox);
            string postBackScript = Page.ClientScript.GetPostBackEventReference(options);

            return string.Format("if (event.keyCode == 10 || event.keyCode == 13){{ if(isValidPage('{0}', '{1}')) {{ {2} }} return false; }}", pageTextBox.ClientID, pageCount.ClientID, postBackScript); 
        }

        protected void ReportNavigation_PreRender(object sender, EventArgs e)
        {
            _pagingState.PersistCurrentPage();
            int totalPages = _pagingState.TotalPages;

            if (totalPages == 1)
            {
                NavigationPanel.Visible = false;
            }
            else
            {
                NavigationPanel.Visible = true;

                int currentPage = Math.Max(_pagingState.CurrentPage, 1);


                PageNumberTextBox.Text = currentPage.ToString();
                PageNumberTextBox.Attributes.Add("onkeypress", GetPagingScript(PageNumberTextBox, PageCountHidden));

                PageCountHidden.Value = totalPages.ToString();
                TotalPages.Text = totalPages.ToString();
            
                FirstPage.Enabled = (currentPage > 1);
                PreviousPage.Enabled = (currentPage > 1);
                NextPage.Enabled = currentPage < totalPages;
                LastPage.Enabled = currentPage < totalPages;

                if (currentPage == 1)
                {
                    FirstPage.Visible = false;
                    FirstPagePassive.Visible = true;
                    PreviousPage.Visible = false;
                    PreviousPagePassive.Visible = true;
                }
                else
                {
                    FirstPage.Visible = true;
                    FirstPagePassive.Visible = false;
                    PreviousPage.Visible = true;
                    PreviousPagePassive.Visible = false;
                }
                if (currentPage == totalPages)
                {
                    NextPage.Visible = false;
                    NextPagePassive.Visible = true;
                    LastPage.Visible = false;
                    LastPagePassive.Visible = true;
                }
                else
                {
                    NextPage.Visible = true;
                    NextPagePassive.Visible = false;
                    LastPage.Visible = true;
                    LastPagePassive.Visible = false;
                }
            }
        }

        /*
     * The report viewer control was resetting its CurrentPage when set from our custom paging control's textbox. This happens
     * for some reason only the first time. This class assigns the current page to the report viewer in OnPrerender which works.
     */
        private class PagingState
        {
            private HiddenField _currentPage;
            private HiddenField _dirtyPage;
            private ReportViewer _reportViewer;
        
            public PagingState(ReportNavigation navigationControl)
            {
                _reportViewer = (ReportViewer)navigationControl.Parent.FindControl(navigationControl.ReportViewerID);
                _currentPage = (HiddenField)navigationControl.Parent.FindControl(navigationControl.CurrentPageControlID);
                _dirtyPage = (HiddenField)navigationControl.Parent.FindControl(navigationControl.DirtyPageID);

                _dirtyPage.Value = bool.FalseString;
                InitCurrentPage();
            }

            private void InitCurrentPage()
            {
                _currentPage.Value = _reportViewer.CurrentPage.ToString();
            }

            private bool IsDirty
            {
                get
                {
                    return bool.Parse(_dirtyPage.Value);
                }
                set
                {
                    _dirtyPage.Value = value.ToString();
                }
            }

            public int TotalPages
            {
                get
                {
                    if (_reportViewer.ProcessingMode.Equals(ProcessingMode.Local))
                    {
                        return _reportViewer.LocalReport.GetTotalPages();
                    }
                    else
                    {
                        return _reportViewer.ServerReport.GetTotalPages();
                    }
                }
            }
        
            public int CurrentPage
            {
                get
                {
                    return int.Parse(_currentPage.Value);
                }
                set
                {
                    IsDirty = true;
                    _currentPage.Value = value.ToString();
                }
            }

            public void PersistCurrentPage()
            {
                //invalid page number can be left inside the textbox when changing reports.
                if ((CurrentPage < 1 || CurrentPage > TotalPages)) 
                    IsDirty = false;

                if (!IsDirty)
                {
                    InitCurrentPage();
                }
                else
                {
                    if (_reportViewer.CurrentPage != CurrentPage)
                        _reportViewer.CurrentPage = CurrentPage;
                }
            }

        }
    }
}
