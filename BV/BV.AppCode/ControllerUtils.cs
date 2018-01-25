using System.Web.UI;

namespace BV.AppCode
{
    public static class ControllerUtils
    {
        public static Control FindControlRecursive(Control root, string id)
        {
            if (root is DataSourceControl)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(root.ID) && root.ID.Equals(id))
            {
                return root;
            }

            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null)
                {
                    return t;
                }
            }
            return null;
        }

        public static string FindTimePeriodId(int period)
        {
            switch (period)
            {
                case 1:
                    return "4weeks";

                case 2:
                    return "8weeks";

                case 3:
                    return "13weeks";

                case 4:
                    return "26weeks";

                case 13:
                    return "avgYTD";

                case 6:
                    return "curMonth";

                case 7:
                    return "avgPriorMonth";

                case 12:
                    return "avgPrior3Month";

                default:
                    return "4weeks";

            }
        }
    }
}