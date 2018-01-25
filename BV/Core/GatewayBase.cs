using System;
using System.Globalization;
using System.Text;
using VB.Common.Core.Cache;
using VB.Common.Core.Data;
using VB.Common.Core.Registry;

namespace VB.Common.Core
{
    public abstract class GatewayBase
    {
        protected string CreateCacheKey(params object[] parameters)
        {
            StringBuilder sb = new StringBuilder("u", 1024);

            sb.Append(GetType().GetHashCode().ToString(CultureInfo.InvariantCulture));

            if (parameters.Length > 0)
            {
                sb.Append("?");

                foreach (object parameter in parameters)
                {
                    sb.Append(parameter);
                    sb.Append("&");
                }

                sb.Length = sb.Length - 1;
            }
            
            return sb.ToString();
        }

        protected Cache.ICache Cache
        {
            get
            {
                return RegistryFactory.GetRegistry().Resolve<Cache.ICache>();
            }
        }

        protected T Resolve<T>()
        {
            return RegistryFactory.GetResolver().Resolve<T>();
        }

        protected IDataSession Session
        {
            get
            {
                IDataSessionManager manager = Resolve<IDataSessionManager>();

                if (manager == null)
                {
                    throw new InvalidOperationException("DataSessionManager not registered");
                }

                return manager.Session;
            }
        }

        /// <summary>
        /// Cache.Remove
        /// </summary>
        /// <param name="key"></param>
        protected void Forget(string key)
        {
            Cache.Remove(key);
        }

        /// <summary>
        /// Cache.Add
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void Remember(string key, object value)
        {
            if (value != null)
            {
                Cache.Add(
                    key,
                    value,
                    CacheConstants.NoAbsoluteExpiration,
                    CacheConstants.DefaultSlidingExpiration);
            }
        }
    }
}