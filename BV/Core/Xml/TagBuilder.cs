using System.Collections.Generic;
using System.Text;

namespace VB.Common.Core.Xml
{
    public class TagBuilder
    {       
        /// <summary>
        /// Create an element named according to the tag variable.  The element will contain the sub-elements
        /// passed in the params array.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Wrap(string tag, params string[] values)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string value in values)
            {
                sb.Append( value ); // not escaped due to nesting.
            }

            string combinedValues = sb.ToString();

            return Wrap(tag, combinedValues);
        }

        private static bool IsIllegalChar(int c)
        {
            return (c >= 0x0 && c <= 0x1F) || c == 0x7F;
        }

        private static string RemoveASCIIControlCharacters(string value)
        {
            StringBuilder builder = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                if (!IsIllegalChar(c))
                    builder.Append(c);
            }

            return builder.ToString();
        }

        private static string Escape(string value)
        {
            // If the value is already a sequence of nodes, we should only escape the values and attribute values.
            // The simple approach is to look at the first and last chararacters.  If they are < and >, we will assume xml.
            if (value.StartsWith("<") && value.EndsWith(">"))
            {
                // An xml node.  Assume already escaped.
                return value;
            }

            // Otherwise, we can escape the whole thing.
/*
            &#38;    &
            &#60;    <
            &#62;    >
            &#34;    "
            &#39;    '
*/
            value = value.Replace("&", "&#38;");
            value = value.Replace("<", "&#60;");
            value = value.Replace(">", "&#62;");
            value = value.Replace("\"", "&#34;");
            value = value.Replace("'", "&#39;");

            return RemoveASCIIControlCharacters(value);
        }

        /// <summary>
        /// Create an element named according to the tag variable.  The attributes dictionary will render
        /// the element's attributes.  The value variable will specify the element's value.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="attributes"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Wrap(string tag, IDictionary<string, string> attributes, params string[] values)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("<");
            sb.Append(tag);

            foreach (string key in attributes.Keys)
            {
                sb.Append(" ");
                sb.Append(key);
                sb.Append("=");
                sb.Append('"' + Escape(attributes[key]) + '"');
            }

            sb.Append(">");
            
            foreach (string value in values)
            {                
                sb.Append( Escape(value) );
            }

            sb.Append("</");
            sb.Append(tag);
            sb.Append(">");

            return sb.ToString();
        }

       
        /// <summary>
        /// Create a tag with the specified value.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Wrap(string tag, string value)
        {
            IDictionary<string, string> emptyDictionary = new Dictionary<string, string>();
            return Wrap(tag, emptyDictionary, value);
        }
      
    }
}
