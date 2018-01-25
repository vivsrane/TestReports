using System;

namespace VB.Common.Core
{
    /// <summary>
    /// A cache abstraction.  
    /// </summary>
    public interface ICache
    {
        #region Cache

        /// <summary>
        /// Set (insert or update) the value in the cache with the specified key.
        /// </summary>
        /// <param name="key">Unique key to identify the value in the cache.  Should not exceed 250 characters.</param>
        /// <param name="value">The value to store in the cache.  Should not exceed 1 MB in size.</param>         
        /// <param name="secondsToExpire">How many seconds until the cache entry expires? Pass null if no expiration.</param>
        void Set(string key, object value, int? secondsToExpire);

        /// <summary>
        /// Set (insert or update) the value in the cache with the specified key.
        /// </summary>
        /// <param name="key">Unique key to identify the value in the cache.  Should not exceed 250 characters.</param>
        /// <param name="value">The value to store in the cache.  Should not exceed 1 MB in size.</param>                 
        void Set(string key, object value);

        /// <summary>
        /// sliding expiration
        /// </summary>
        void Set(string key, object value, TimeSpan sliding);

        /// <summary>
        /// Retrieve the object from the cache identified with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object Get(string key);

        /// <summary>
        /// Delete the object from the cache identified with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The deleted object.</returns>
        object Delete(string key);
        
        #endregion
    }
}