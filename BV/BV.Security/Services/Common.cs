using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;


namespace BV.Security.Services
{
    public static class Common
    {
		/// <summary>
		/// Returns an individual HTTP Header value
		/// </summary>
		/// <param name="request"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetHeader(this HttpRequestMessage request, string key)
		{
			IEnumerable<string> keys;
			if (!request.Headers.TryGetValues(key, out keys))
				return null;

			return keys.First();
		}

		/// <summary>
		/// Retrieves an individual cookie from the cookies collection
		/// </summary>
		/// <param name="request"></param>
		/// <param name="cookieName"></param>
		/// <returns></returns>
		public static string GetCookie(this HttpRequestMessage request, string cookieName)
		{
			var cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
			if (cookie != null)
				return cookie[cookieName].Value;

			return null;
		}

		public static string GetClientIp(HttpRequestMessage request)
		{
			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
			}

			return null;
		}

		public static string GetUserHostName(HttpRequestMessage request)
		{
			if (request.Properties.ContainsKey("MS_HttpContext"))
			{
				return ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostName;
			}

			return null;
		}
	}
}
