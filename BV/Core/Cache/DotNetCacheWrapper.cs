using System;
using System.Web;

namespace VB.Common.Core.Cache
{
    /// <summary>
    /// An implementation of ICache wrapping the .net HttpContext.Cache
    /// </summary>
    public class DotNetCacheWrapper : Core.ICache
    {
        /// <summary>
        /// Set (insert or update) the value in the cache with the specified key.
        /// </summary>
        /// <param name="key">Unique key to identify the value in the cache.  Should not exceed 250 characters.</param>
        /// <param name="value">The value to store in the cache.  Should not exceed 1 MB in size.</param>         
        /// <param name="secondsToExpire">How many seconds until the cache entry expires? Pass null if no expiration.</param>
        public void Set(string key, object value, int? secondsToExpire)
        {
            // if no value specified, do nothing.
            if (value == null) return;

            // TODO: Replace with a proper cache - e.g. memcached.
            if (key.Length > 250)
            {
                throw new ApplicationException("Cache keys must not exceed 250 characters.");
            }

            if (secondsToExpire.HasValue)
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Cache.Insert(key, value, null, DateTime.UtcNow.AddSeconds(secondsToExpire.Value), System.Web.Caching.Cache.NoSlidingExpiration);

            }
            else
            {
                if (HttpContext.Current != null)
                    HttpContext.Current.Cache.Insert(key, value);
            }

        }

        //sliding expiration
        public void Set(string key, object value, TimeSpan sliding)
        {
            // if no value specified, do nothing.
            if (value == null) return;

            if (key.Length > 250)
            {
                throw new ApplicationException("Cache keys must not exceed 250 characters.");
            }

            if (HttpContext.Current != null)
                HttpContext.Current.Cache.Insert(key, value, null, System.Web.Caching.Cache.NoAbsoluteExpiration, sliding);           
        }

        public void Set(string key, object value)
        {
            Set(key, value, null);
        }

        public object Get(string key)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Cache.Get(key);

            return null;
        }

        public object Delete(string key)
        {
            if (HttpContext.Current != null)
                return HttpContext.Current.Cache.Remove(key);
            
            return null;
        }
    }
}
