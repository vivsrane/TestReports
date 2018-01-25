using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Xml;

namespace VB.Common.Core.Data
{
    public static class DataRecord
    {
        public static string GetString(this IDataRecord record, string columnName)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (record.IsDBNull(ordinal))
            {
                return String.Empty;
            }
            return record.GetString(ordinal);
        }

        public static short GetTinyInt(this IDataRecord record, string columnName, Int16 defaultValue)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (record.IsDBNull(ordinal))
            {
                return defaultValue;
            }
            return Convert.ToInt16(record.GetValue(ordinal));
        }

        public static double GetDouble(this IDataRecord record, string columnName, double defaultValue)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (record.IsDBNull(ordinal))
            {
                return defaultValue;
            }
            return Convert.ToDouble(record.GetValue(ordinal));
        }

        public static bool GetBoolean(this IDataRecord record, string columnName, bool defaultValue)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (record.IsDBNull(ordinal))
            {
                return defaultValue;
            }
            return Convert.ToBoolean(record.GetValue(ordinal));
        }


        public static int? GetNullableInt32(this IDataRecord record, string columnName)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (record.IsDBNull(ordinal))
            {
                return null;
            }
            return record.GetInt32(ordinal);
        }

        public static XmlDocument GetXmlDocument(this IDataRecord record, string columnName)
        {
            SqlDataReader reader = record as SqlDataReader;

            XmlDocument document = new XmlDocument();

            if (reader != null)
            {
                SqlXml sqlXml = reader.GetSqlXml(record.GetOrdinal(columnName));

                using (XmlReader xmlReader = sqlXml.CreateReader())
                {
                    document.Load(xmlReader);

                    return document;
                }
            }

            return document;
        }

        public static string ToXml(XmlDocument document)
        {
            return document.OuterXml;
        }

        public static bool HasColumn(this IDataRecord dr, string columnName)
        {
            for (var i = 0; i < dr.FieldCount; i++)
            {
                if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public static int GetInt32(this IDataRecord record, string columnName, int defaultValue)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (!record.IsDBNull(ordinal))
            {
                return record.GetInt32(ordinal);
            }

            return defaultValue;
        }

        public static Guid GetGuid(this IDataRecord record, string columnName, Guid defaultValue)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (!record.IsDBNull(ordinal))
            {
                return record.GetGuid(ordinal);
            }

            return defaultValue;
        }

        public static decimal GetDecimal(this IDataRecord record, string columnName, decimal defaultValue)
        {
            int ordinal = record.GetOrdinal(columnName);
            if (!record.IsDBNull(ordinal))
            {
                return record.GetDecimal(ordinal);
            }

            return defaultValue;
        }


        public static bool GetBoolean(this IDataRecord record, string columnName)
        {
            int ordinal = record.GetOrdinal(columnName);

          
            return record.GetBoolean(ordinal);
        }

        public static DateTime GetDateTime(this IDataRecord record, string columnName)
        {
            int ordinal = record.GetOrdinal(columnName);

            if (record.IsDBNull(ordinal))
            {
                return DateTime.MinValue;
            }
            return record.GetDateTime(ordinal);
        }
    }
}