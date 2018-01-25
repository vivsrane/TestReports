using System;
using System.Collections.Generic;
using System.Linq;

namespace VB.Reports.App.ReportDefinitionLibrary
{
    public static class ReportAnalyticsClient
    {
        //private static readonly ILog Log = LogManager.GetLogger(typeof(ReportAnalyticsClient).FullName);

        //private static readonly ICacheKeyBuilder CacheKeyBuilder = new CacheKeyBuilder();
        //private static readonly ICacheWrapper CacheWrapper = new MemcachedClientWrapper();
        private const int CacheHours = 1;

        public static TValue GetReportData<TValue>(List<string> key) where TValue : class
        {
            string typeName = typeof(TValue).Name;
            key.Add(typeName);
            //string cacheKey = CacheKeyBuilder.CacheKey(key);

            // Check cache.
            //var obj = CacheWrapper.Get<TValue>(cacheKey);

            //if (obj != null)
            //{
            //    Log.Debug("cache.hit");
            //}
            //else
            //{
            //    Log.Debug("cache.miss");    
            //}

            // return obj;
            return null;
        }

        public static void SetReportData<T>(List<string> key, T data) where T : class
        {
            string typeName = typeof(T).Name;
            key.Add(typeName);
            //string cacheKey = CacheKeyBuilder.CacheKey(key);

            // Put it in the cache for an hour
           // CacheWrapper.Set(cacheKey, data, DateTime.Now.AddHours(CacheHours));
        }

        public static List<string> GetParameterList(Dictionary<string, object> parameterValues)
        {
            List<string> lstParameterList = parameterValues.Select(x => Convert.ToString(x.Value)).ToList<string>();
            return lstParameterList;
        }       
    }
}
