using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace VB.Common.Core.Utilities
{
    public static class EnumHelper
    {
        /// <summary>
        /// This method probably belongs in either VB.Common.Data or Util.
        /// 
        /// Converts enums into Name, Value pairs as columns in a datatable.
        /// </summary>
        /// <returns>A DataTable with the enum names put in the Name column replacing underscores with spaces and the enum value into a Value column.</returns>
        public static DataTable EnumToDataTable(Type enumType)
        {
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            Type underlyingType = Enum.GetUnderlyingType(enumType);

            DataTable datatable = new DataTable();
            datatable.Locale = CultureInfo.InvariantCulture;

            DataColumn nameColumn = new DataColumn("Name", Type.GetType("System.String"));
            datatable.Columns.Add(nameColumn);

            DataColumn valueColumn = new DataColumn("Value", underlyingType);
            datatable.Columns.Add(valueColumn);

            Array values = Enum.GetValues(enumType);

            for (int i = 0; i < values.Length; i++)
            {
                object enumValue = values.GetValue(i);

                object underlyingValue = Convert.ChangeType(enumValue, underlyingType, CultureInfo.InvariantCulture);

                DataRow row = datatable.NewRow();
                row[nameColumn] = GetEnumDescription(enumType, (Enum)enumValue);
                row[valueColumn] = underlyingValue;

                datatable.Rows.Add(row);
            }

            return datatable;
        }

        public static T GetEnumFromDescription<T>(string value)
            where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Unable to find enum field for empty or null string");
            }

            var type = typeof (T);

            if (!type.IsEnum)
            {
                throw new ArgumentException("Generic must be of Enum type", "Generic");
            }

            var fields = type.GetFields();
            foreach (var f in fields)
            {
                var attrs = (DescriptionAttribute[]) f.GetCustomAttributes(typeof (DescriptionAttribute), false);

                if (attrs.Length > 0 && value.Equals(attrs[0].Description))
                {
                    var enumValue = (T)f.GetValue(null);
                    return enumValue;
                }
                
                if (f.Name.Equals(value))
                {
                    var enumValue = (T)f.GetValue(null);
                    return enumValue;
                }
            }
            
            throw new ArgumentException("Unable to find enum field from provided string");
        }

        public static string GetEnumDescription(Type enumType, Enum value)
        {
            FieldInfo fi = enumType.GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                                            typeof(DescriptionAttribute),
                                            false);

            if (attributes != null && attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static string GetDescription<T>(this T enumerationValue)
            where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }
            return GetEnumDescription(type, enumerationValue as Enum);
        }

        /// <summary>
        /// Get a typed array of values from an Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Array of all Enum values of Type T</returns>
        public static IEnumerable<T> GetValues<T>()
        {
            Type enumType = typeof(T);

            // Can't use generic type constraints on value types,
            // so have to do check like this
            if (enumType.BaseType != typeof(Enum))
                throw new ArgumentException("T must be of type System.Enum");

            return (T[])Enum.GetValues(typeof(T));
        }
        
    }
}