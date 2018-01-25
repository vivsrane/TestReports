using System.Collections.Generic;

namespace VB.Common.Core.Utilities
{
    public static class CollectionHelper
    {
        public static T First<T>(IEnumerable<T> values)
        {
            IEnumerator<T> en = values.GetEnumerator();

            return (en.MoveNext()) ? en.Current : default(T);
        }
    }
}