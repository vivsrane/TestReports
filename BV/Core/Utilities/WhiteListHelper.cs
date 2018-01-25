using System;
using System.Collections.Generic;
using System.Linq;

namespace VB.Common.Core.Utilities
{
    public static class WhiteListHelper
    {
        public static bool IsInWhiteList(string value, string whiteList, StringComparer comparer = null, 
            string allValue = "ALL", bool emptyWhitelistAllowsAll = false, char whiteListSeparator=',')
        {
            if (emptyWhitelistAllowsAll && string.IsNullOrWhiteSpace(whiteList))
                return true;

            if(whiteList == null)
                throw new ArgumentNullException("whiteList");

            return IsInWhiteList(value,
                                 whiteList.Split(whiteListSeparator).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()),
                                 (comparer ?? StringComparer.InvariantCultureIgnoreCase).Compare, 
                                 allValue);
        }

        public static bool IsInWhiteList<T>(T value, IEnumerable<T> whiteList, Func<T, T, int> comparer, T allValue, bool emptyWhitelistAllowsAll = false)
        {
            var whiteListArray = (whiteList == null) ? new T[0] : whiteList.ToArray();
            if (emptyWhitelistAllowsAll && ( whiteList == null || !whiteListArray.Any() || whiteListArray.All(wl => string.IsNullOrWhiteSpace(wl.ToString()))))
                return true;

            if (whiteList == null)
                throw new ArgumentNullException("whiteList");

            comparer = comparer ?? Comparer<T>.Default.Compare;
            
            return whiteListArray.Any(v => comparer(allValue, v) == 0 || comparer(value, v) == 0);
        }
    }
}