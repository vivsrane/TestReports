using Newtonsoft.Json;

namespace VB.Common.Core.Extensions
{
    public static class JsonExtensions 
    {
        /// <summary>
        /// Serializes a type to Json.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>       
        public static string ToJson(this object value) 
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// Deserializes a json string into a specific type.
        /// Note that the type specified must be serializable.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string json)
        {
            object obj = JsonConvert.DeserializeObject(json, typeof (T));
            return (T) obj;
        }
    }
}
