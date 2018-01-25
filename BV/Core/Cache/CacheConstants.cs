using System;

namespace VB.Common.Core.Cache
{
    public static class CacheConstants
    {
        public static readonly DateTime NoAbsoluteExpiration = DateTime.MaxValue;

        public static readonly TimeSpan NoSlidingExpiration = TimeSpan.Zero;

        public static TimeSpan DefaultSlidingExpiration
        {
            get
            {
                return new TimeSpan(0,30,0); // TODO: Take from ConfigurationManager
            }
        }
        public static TimeSpan SlidingExpiration
        {
            get
            {
                return new TimeSpan(2, 0, 0); // TODO: Take from ConfigurationManager
            }
        }
    }
}