using System;
using System.Data;
using System.Globalization;

namespace VB.Common.Core.Utilities
{
    public static class ConversionHelper
    {
        /// <summary>
        /// Convert the given value into an instance of the given type.  If the object is null
        /// (literally or DbNull) then the default value is returned.
        /// </summary>
        /// <param name="value">Value to be type converted</param>
        /// <param name="targetType">Target type</param>
        /// <param name="defaultValue">Default returned for null values</param>
        /// <returns></returns>
        public static object Convert(object value, Type targetType, object defaultValue)
        {
            if (value != null && DBNull.Value.Equals(value))
            {
                return defaultValue;
            }

            object conversion = Convert(value, targetType);

            if (conversion == null)
            {
                conversion = defaultValue;
            }

            return conversion;
        }

        /// <see cref="Convert(object,Type,bool)"/>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object Convert(object value, Type targetType)
        {
            return Convert(value, targetType, false);
        }

        /// <summary>
        /// Convert the argument <paramref name="value"/> into the <paramref name="targetType"/>.
        /// If the value is null and the target type is not nullable then we throw an exception if
        /// <paramref name="throwException"/> is true, otherwise we return null.  In all other cases
        /// a best attempt to perform the conversion is made.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="throwException"></param>
        /// <returns></returns>
        public static object Convert(object value, Type targetType, bool throwException)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException("targetType");
            }

            bool isNullable = IsNullable(targetType);

            if (value == null)
            {
                if (Type.GetTypeCode(targetType).Equals(TypeCode.Object))
                {
                    return null;
                }
                else
                {
                    if (isNullable || !throwException)
                    {
                        return null;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(
                            string.Format(CultureInfo.CurrentCulture, "Cannot convert null to value type {0}", targetType));
                    }
                }
            }

            object conversion = null;

            if (value.GetType().Equals(targetType))
            {
                conversion = value;
            }
            else
            {
                Type type = targetType;

                if (isNullable)
                {
                    type = Nullable.GetUnderlyingType(type);
                }

                IConvertible convertible = value as IConvertible;

                if (convertible != null)
                {
                    conversion = convertible.ToType(type, CultureInfo.CurrentCulture);

                    if (isNullable)
                    {
                        conversion = ToNullable(type, conversion);
                    }
                }
            }

            return conversion;
        }

        public static Type ToType(DbType type)
        {
            Type result = typeof (Int32);

            switch (type)
            {
                case DbType.Boolean:
                    result = typeof (Boolean);
                    break;
                case DbType.DateTime:
                    result = typeof (DateTime);
                    break;
                case DbType.Double:
                    result = typeof (Decimal);
                    break;
                case DbType.Int32:
                    result = typeof (Int32);
                    break;
                case DbType.String:
                    result = typeof (String);
                    break;
            }

            return result;
        }

        public static bool IsNullable(Type type)
        {
            return (Nullable.GetUnderlyingType(type) != null);
        }

        /// <summary>
        /// Switches on the types <code>TypeCode</code> to produce a correctly typed generic Nullable
        /// instance for the caller.  The the type is an object (i.e. a value-type) then null is returned.
        /// </summary>
        /// <param name="type">The type of nullable desired.</param>
        /// <param name="value">The value to be wrapped in a <code>Nullable&lt;T&gt;</code> instance</param>
        /// <returns></returns>
        public static ValueType ToNullable(Type type, object value)
        {
            ValueType nullable = null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    nullable = (value == null) ? new Nullable<bool>() : new Nullable<bool>((bool) value);
                    break;
                case TypeCode.Byte:
                    nullable = (value == null) ? new Nullable<byte>() : new Nullable<byte>((byte)value);
                    break;
                case TypeCode.Char:
                    nullable = (value == null) ? new Nullable<char>() : new Nullable<char>((char)value);
                    break;
                case TypeCode.DateTime:
                    nullable = (value == null) ? new Nullable<DateTime>() : new Nullable<DateTime>((DateTime)value);
                    break;
                case TypeCode.Decimal:
                    nullable = (value == null) ? new Nullable<decimal>() : new Nullable<decimal>((decimal)value);
                    break;
                case TypeCode.Double:
                    nullable = (value == null) ? new Nullable<double>() : new Nullable<double>((double)value);
                    break;
                case TypeCode.Int16:
                    nullable = (value == null) ? new Nullable<short>() : new Nullable<short>((short)value);
                    break;
                case TypeCode.Int32:
                    nullable = (value == null) ? new Nullable<int>() : new Nullable<int>((int)value);
                    break;
                case TypeCode.Int64:
                    nullable = (value == null) ? new Nullable<long>() : new Nullable<long>((long)value);
                    break;
                case TypeCode.SByte:
                    nullable = (value == null) ? new Nullable<sbyte>() : new Nullable<sbyte>((sbyte)value);
                    break;
                case TypeCode.Single:
                    nullable = (value == null) ? new Nullable<float>() : new Nullable<float>((float)value);
                    break;
                case TypeCode.UInt16:
                    nullable = (value == null) ? new Nullable<ushort>() : new Nullable<ushort>((ushort)value);
                    break;
                case TypeCode.UInt32:
                    nullable = (value == null) ? new Nullable<uint>() : new Nullable<uint>((uint)value);
                    break;
                case TypeCode.UInt64:
                    nullable = (value == null) ? new Nullable<ulong>() : new Nullable<ulong>((ulong)value);
                    break;
            }

            return nullable;
        }

        /// <summary>
        /// If the type is nullable then we return <code>DbNull.Value</code> otherwise we
        /// switch on the <code>TypeCode</code> and get the correct value.  This method is
        /// used in conjunction with the DataTable, whose cells cannot contain the
        /// <code>null</code> value which is why we use <code>DbNull.Value</code>.
        /// </summary>
        /// <param name="type">Type for which we want a default value</param>
        /// <param name="nullable">Whether the type is declared as being nullable</param>
        /// <returns>The default value for the type</returns>
        public static object DefaultValue(Type type, bool nullable)
        {
            bool isObjectType = Type.GetTypeCode(type).Equals(TypeCode.Object);

            if (nullable || isObjectType)
                return DBNull.Value;

            object defaultValue = null;

            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    defaultValue = default(bool);
                    break;
                case TypeCode.Byte:
                    defaultValue = default(byte);
                    break;
                case TypeCode.Char:
                    defaultValue = default(char);
                    break;
                case TypeCode.DateTime:
                    defaultValue = default(DateTime);
                    break;
                case TypeCode.Decimal:
                    defaultValue = default(decimal);
                    break;
                case TypeCode.Double:
                    defaultValue = default(double);
                    break;
                case TypeCode.Int16:
                    defaultValue = default(short);
                    break;
                case TypeCode.Int32:
                    defaultValue = default(int);
                    break;
                case TypeCode.Int64:
                    defaultValue = default(long);
                    break;
                case TypeCode.SByte:
                    defaultValue = default(sbyte);
                    break;
                case TypeCode.Single:
                    defaultValue = default(float);
                    break;
                case TypeCode.UInt16:
                    defaultValue = default(ushort);
                    break;
                case TypeCode.UInt32:
                    defaultValue = default(uint);
                    break;
                case TypeCode.UInt64:
                    defaultValue = default(ulong);
                    break;
            }

            return defaultValue;
        }
    }
}