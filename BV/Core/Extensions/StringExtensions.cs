using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace VB.Common.Core.Extensions
{
    public static class StringExtensions
    {
        public static string ToProperCase(this string text)
        {
            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            return textInfo.ToTitleCase(text);
        }

        public static string Right(this string s, int len)
        {
            int start = s.Length - len;
            return (len < s.Length) ? s.Substring(start, len) : s;
        }

        public static string FirstCharacterToLower(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsLower(str, 0))
                return str;

            return Char.ToLowerInvariant(str[0]).ToString() + str.Substring(1);
        }

        public static string FirstCharacterToUpper(this string str)
        {
            if (String.IsNullOrEmpty(str) || Char.IsUpper(str, 0))
                return str;

            return Char.ToUpperInvariant(str[0]) + str.Substring(1);
        }

        public static IEnumerable<string> ToEnumerable(this string str, char delimiter = ',')
        {
            return str.Split(delimiter).ToArray();
        }

        public static string SmartTruncate(this string sText, int iLength, char breakOnChar = ' ', bool breakBefore = true, bool addTextOverflowEllipsis = false, bool forceTrancate=false)
        {
            if(string.IsNullOrEmpty(sText))
                return sText;

            string sReturn = sText.Trim();

            if (!String.IsNullOrEmpty(sReturn) && sReturn.Length > iLength)
            {
                if (sReturn.Substring(iLength, 1) != " ") // character past length is not space?
                {
                    int iPos;
                    if (breakBefore)
                        // find the last space before the target length
                        iPos = sReturn.LastIndexOf(breakOnChar, iLength);
                    else
                        // find the next space after the target length
                        iPos = sReturn.Substring(iLength).IndexOf(breakOnChar) + iLength;

                    if (iPos > 0)
                    {
                        sReturn = sReturn.Substring(0, iPos);
                    }
                    else
                    {
                        if (forceTrancate)
                        {
                            sReturn = sReturn.Substring(0, iLength);//Force text truncate here if we can not find breakOnChar before iLength
                        }
                    }

                    if (addTextOverflowEllipsis)
                    {
                        sReturn = sReturn + "...";
                    }
                }
                else
                    sReturn = sReturn.Substring(0, iLength);
                
                
            }

            return sReturn;
        }
     
    }
}
