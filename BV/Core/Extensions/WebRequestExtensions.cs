using System.Configuration;
using System.Net;

namespace VB.Common.Core.Extensions
{
    public static class WebRequestExtensions
    {
        public static CredentialCache GetConfiguredCredentials(this WebRequest request, string configUserKey, string configPasswordKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            var credentialCache = new CredentialCache
            {
                {
                    request.RequestUri,
                    "Basic",
                    new NetworkCredential(
                        ConfigurationManager.AppSettings[configUserKey],
                        ConfigurationManager.AppSettings[configPasswordKey])
                }
            };
            return credentialCache;
        }
    }
}