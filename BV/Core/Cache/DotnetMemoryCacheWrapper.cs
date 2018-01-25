using System;
using System.Runtime.Caching;
using System.Threading;

namespace VB.Common.Core.Cache
{
    public class DotnetMemoryCacheWrapper : IMemoryCache
    {
        private readonly ObjectCache InnerCache;

        private static readonly Lazy<ObjectCache> LazyDefaultCache = new Lazy<ObjectCache>(
            () => new System.Runtime.Caching.MemoryCache("DotnetMemoryCacheWrapper"),
            LazyThreadSafetyMode.PublicationOnly);

        public DotnetMemoryCacheWrapper(ObjectCache cache)
        {
            InnerCache = cache;
        }

        public DotnetMemoryCacheWrapper()
            : this(LazyDefaultCache.Value)
        {
        }

        public void Set(string key, object value, int? secondsToExpire)
        {
            var item = new CacheItem(key, value);
            var policy = new CacheItemPolicy();
            if (secondsToExpire.HasValue)
                policy.AbsoluteExpiration = DateTimeOffset.UtcNow.AddSeconds(secondsToExpire.Value);
            InnerCache.Set(item, policy);
        }

        public void Set(string key, object value)
        {
            Set(key, value, null);
        }

        public void Set(string key, object value, TimeSpan sliding)
        {
            var item = new CacheItem(key, value);
            var policy = new CacheItemPolicy {SlidingExpiration = sliding};
            InnerCache.Set(item, policy);
        }

        public object Get(string key)
        {
            return InnerCache.Get(key);
        }

        public object Delete(string key)
        {
            return InnerCache.Remove(key);
        }
    }
}