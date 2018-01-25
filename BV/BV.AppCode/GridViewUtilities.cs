using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BV.AppCode
{
    public static class GridViewUtilities
    {
        public static void MergeColumnHeaderCell(GridViewRow row, string columnText)
        {
            if (row.RowType.Equals(DataControlRowType.Header))
            {
                int columnIndex = -1;

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    if (row.Cells[i].Text.Equals("MergeColumnHeaderCell"))
                    {
                        columnIndex = i;
                        break;
                    }
                }

                if (columnIndex != -1)
                {
                    row.Cells.RemoveAt(columnIndex);
                    row.Cells[columnIndex - 1].ColumnSpan = 2;
                }
            }
        }

        public static void HighlightCells(GridViewRow row, int[] highlightColumns)
        {
            for (int i = 0; i < highlightColumns.GetLength(0); i++)
            {
                row.Cells[highlightColumns[i]].CssClass += " light";
            }
            row.Cells[row.Cells.Count - 1].CssClass += " last";
        }

        public static GridViewRow CopyRow(GridViewRow inputRow, DataControlRowType rowType, GridView gridView)
        {
            GridViewRow r = new GridViewRow(
                -1, -1,
                rowType, DataControlRowState.Normal);

            for (int i = 0; i < inputRow.Cells.Count; i++)
            {
                Object colType = gridView.Columns[i];
                if (inputRow.Cells[i] is DataControlFieldCell)
                {
                    r.Cells.Add(new DataControlFieldCell(((DataControlFieldCell) inputRow.Cells[i]).ContainingField));
                }
                else
                {
                    r.Cells.Add(new TableCell());
                }


                if (!(inputRow.Parent.Parent as GridView).Columns[i].Visible)
                {
                    r.Cells[i].Visible = (inputRow.Parent.Parent as GridView).Columns[i].Visible;
                    continue;
                }
                else if (colType is HyperLinkField)
                {
                    r.Cells[i].Text = (inputRow.Cells[i].Controls[0] as HyperLink).Text;
                }
                else if (colType is TemplateField)
                {
                    StringBuilder stringBuilder = new StringBuilder();
                    HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(stringBuilder));
                    inputRow.Cells[i].RenderControl(writer);
                    string body = stringBuilder.ToString();

                    // get rid of superfluous <td> tags and disable links

                    Match m = new Regex("td class=\"([^\"]*)\"", RegexOptions.IgnoreCase).Match(body);
                    if (m.Success)
                    {
                        if (r.Cells[i] is DataControlFieldCell)
                        {
                            ((DataControlFieldCell) r.Cells[i]).ContainingField.ItemStyle.CssClass = m.Groups[1].Value;
                        }
                    }

                    body = body.Substring(body.IndexOf('>') + 1, body.Length - (body.IndexOf('>') + 6));

                    // remove any linking business on roll up lines
                    body = body.Replace("onclick=\"", "onclick=\"return false;");
                    body = body.Replace("a href", "a onclick=\"return false;\" href");
                    body = body.Replace("dyna_link", "");

                    r.Cells[i].Text = body;
                }
                else if (colType is BoundField)
                {
                    r.Cells[i].Text = inputRow.Cells[i].Text;
                }
                else
                {
                    throw new Exception("Error: Add a new type handler to GridViewUtilities.cs for type " +
                                        colType.GetType().ToString());
                }
                r.Cells[i].CssClass = inputRow.Cells[i].CssClass;
                r.Cells[i].Visible = (inputRow.Parent.Parent as GridView).Columns[i].Visible;
            }
            return r;
        }

        public static void SortRowsByColumn(List<GridViewRow> sortList, int columnToSortBy)
        {
            sortList.Sort(
                delegate(GridViewRow p1, GridViewRow p2)
                {
                    return p1.Cells[columnToSortBy].Text.CompareTo(p2.Cells[columnToSortBy].Text);
                });
        }

    }
}