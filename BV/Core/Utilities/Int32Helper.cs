using System;
using System.Collections.Specialized;
using System.Globalization;

namespace VB.Common.Core.Utilities
{
    public static class Int32Helper
    {
        public static int ToInt32(object value)
        {
            return ToNullableInt32(value).GetValueOrDefault();
        }

        public static int ToInt32(NameValueCollection parameters, string name)
        {
            return ToNullableInt32(parameters, name).GetValueOrDefault();
        }

        private static int? ToNullableInt32(string text)
        {
            int i; if (Int32.TryParse(text, out i))
                return i;
            return null;
        }

        private static int? ToNullableInt32(IConvertible value)
        {
            try
            {
                return value.ToInt32(CultureInfo.CurrentCulture.NumberFormat);
            }
            catch (InvalidCastException)
            {
                return null;
            }
        }

        public static int? ToNullableInt32(object value)
        {
            int? i;

            if (value == null || value.Equals(DBNull.Value))
            {
                i = null;
            }
            else
            {
                if (value is int)
                {
                    i = (int) value;
                }
                else if (value is int?)
                {
                    i = (int?) value;
                }
                else
                {
                    string str = value as string;

                    if (str != null)
                    {
                        i = ToNullableInt32(str);
                    }
                    else
                    {
                        IConvertible convertible = value as IConvertible;

                        if (convertible != null)
                        {
                            i = ToNullableInt32(convertible);
                        }
                        else
                        {
                            i = ToNullableInt32(value.ToString());
                        }
                    }
                }
            }

            return i;
        }

        public static int? ToNullableInt32(NameValueCollection parameters, string name)
        {
            string[] values = parameters.GetValues(name);
            if (values == null || values.Length == 0)
                return null;
            return ToNullableInt32(values[0]);
        }

        public static string FormatAsOrdinal(object value)
        {
            int? i = ToNullableInt32(value);

            if (i.HasValue)
            {
                return FormatAsOrdinal(i.Value);
            }
            else
            {
                return "N/A";
            }
        }

        public static string FormatAsOrdinal(int value)
        {
            int testValue = (value > 100) ? value % 100 : value;

            if (testValue >= 21) testValue = value % 10;

            string text;

            switch (testValue)
            {
                case 1:
                    text = "st";
                    break;
                case 2:
                    text = "nd";
                    break;
                case 3:
                    text = "rd";
                    break;
                default:
                    text = "th";
                    break;
            }

            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", value, text);
        }
    }
}
