using System;
using System.Text.RegularExpressions;

namespace VB.Common.Core.Utilities
{
	public static class StringHelper
	{
		public static bool CompareStringArrays(string[] stringA, string[] stringB)
		{
			if ((stringA != null) || (stringB != null))
			{
				if ((stringA == null) || (stringB == null))
				{
					return false;
				}
				if (stringA.Length != stringB.Length)
				{
					return false;
				}
				for (int i = 0; i < stringA.Length; i++)
				{
					if (!string.Equals(stringA[i], stringB[i], StringComparison.Ordinal))
					{
						return false;
					}
				}
			}
			return true;
		}

        public static string StripHtmlTags(string inputString)
        {
            return Regex.Replace
              (inputString, "<.*?>", string.Empty);
        }

         

        /// <summary>
        /// Compute Levenshtein distance
        /// </summary>
        /// <param name="s">String 1</param>
        /// <param name="t">String 2</param>
        /// <returns>Distance between the two strings.
        /// The larger the number, the bigger the difference.
        /// </returns>
        /// <see cref="http://www.merriampark.com/ldcsharp.htm"/>
        /// <seealso cref="http://staffwww.dcs.shef.ac.uk/people/S.Chapman/stringmetrics.html#Levenshtein"/>
        public static int LevenshteinDistance(string s, string t)
        {
            // Let's say that null == null is true, but null != non-null
            if (s == null && t == null) return 0;
            if (s == null || t == null) return 1;

            int n = s.Length; //length of s
            int m = t.Length; //length of t
            int[,] d = new int[n + 1, m + 1]; // matrix

            // Step 1
            if (n == 0) return m;
            if (m == 0) return n;

            // Step 2
            for (int i = 0; i <= n; i++)
            {
                d[i, 0] = i;
            }

            for (int j = 0; j <= m; j++)
            {
                d[0, j] = j;
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t.Substring(j - 1, 1) == s.Substring(i - 1, 1) ? 0 : 1); // cost

                    // Step 6
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                              d[i - 1, j - 1] + cost);
                }
            }

            // Step 7
            return d[n, m];
        }


        /// <summary>
        /// A system-wide definition of whether two strings are substantially similar.  If the Levenshtein distance
        /// is less than 25% of the average string length, we consider the strings to be "similar".  25% was just 
        /// an educated guess.  We may want to change that number. 
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool AreSimilar(string lhs, string rhs)
        {
            // Let's say that null == null is true, but null != non-null
            if (lhs == null && rhs == null) return true;
            if (lhs == null || rhs == null) return false;

            // I just pulled 25% out of my hat.  Adjust as needed.
            const double MAX_DISTANCE_PERCENT = 0.25;

            // Compute Levenshtein distance
            int distance = LevenshteinDistance(lhs, rhs);

            // Exact match.
            if (distance == 0)
            {
                return true;
            }

            // If the distance exceeds the % threshold, the strings are different.  Otherwise, they are close enough.
            double averageLength = (lhs.Length + rhs.Length) / 2.0;

            // watch out for divide by zero
            if (averageLength == 0)
            {
                return true;
            }

            double relativeDistance = distance / averageLength;

            return relativeDistance > MAX_DISTANCE_PERCENT ? false : true;
        }

	}



}
