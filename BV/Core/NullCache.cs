using System;

namespace VB.Common.Core
{
    /// <summary>
    /// An ICache implementation which does nothing.
    /// </summary>
    public class NullCache : ICache, Cache.ICache
    {
        public object Add(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            return null;
        }

        public void Set(string key, object value, int? secondsToExpire)
        {
        }

        public void Set(string key, object value)
        {
        }

        public void Set(string key, object value, TimeSpan sliding)
        {
        }

        public object Get(string key)
        {
            return null;
        }

        public object Delete(string key)
        {
            return null;
        }

        #region ICache Members


        public object Remove(string key)
        {
            return null;
        }

        #endregion
    }
}