using System;

namespace VB.Common.Core.Cache
{
    public interface ICache
    {
        object Add(
            string key,
            object value,
            DateTime absoluteExpiration,
            TimeSpan slidingExpiration
            );

        object Get(string key);

        object Remove(string key);
    }
}
