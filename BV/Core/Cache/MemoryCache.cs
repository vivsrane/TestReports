using System;
using System.Collections.Generic;

namespace VB.Common.Core.Cache
{
    /// <summary>
    /// This class is an implementation of ICache that should be registered as a shared
    /// instance so that the cache values persist between calls.
    /// </summary>
    public class MemoryCache : ICache
    {
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();

        public object Add(string key, object value, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            object original = Remove(key);

            _values.Add(key, value);

            return original;
        }

        public object Get(string key)
        {
            if (_values.ContainsKey(key))
            {
                return _values[key];
            }

            return null;
        }

        public object Remove(string key)
        {
            object original;

            if (_values.TryGetValue(key, out original))
            {
                _values.Remove(key);
            }

            return original;
        }
    }
}