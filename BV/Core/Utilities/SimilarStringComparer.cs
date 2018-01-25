using System.Collections.Generic;

namespace VB.Common.Core.Utilities
{
    public class SimilarStringComparer : IEqualityComparer<string>
    {
        #region Implementation of IEqualityComparer<string>

        public bool Equals(string x, string y)
        {
            return StringHelper.AreSimilar(x, y);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }

        #endregion
    }
}
