using System.Collections.Generic;

namespace BV.DomainModel.SoftwareSystem
{
    public static class WebResourceRegistryCache
    {
        private static readonly IDictionary<string, WebResourceRegistry> cache = new Dictionary<string,WebResourceRegistry>();

        public static WebResourceRegistry CacheCopy(string path)
        {
            lock (cache)
            {
                if (cache.ContainsKey(path))
                {
                    return cache[path];
                }
                else
                {
                    WebResourceRegistry registry = new WebResourceRegistry(path);

                    cache[path] = registry;

                    return registry;
                }
            }
        }
    }
}
