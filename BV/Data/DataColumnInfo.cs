using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;

namespace VB.Common.Data
{
    public struct DataColumnInfo
    {
        /// <summary>
        /// Properties of a column schema entry.
        /// </summary>
        /// <see cref="http://msdn2.microsoft.com/en-us/library/system.data.sqlclient.sqldatareader.getschematable(vs.71).aspx"/>
        private static readonly string[] ExtendedProperties = new string[] {
                "ProviderType", "DataTypeName", "NumericPrecision", "NumericScale", "ColumnSize", "IsLong"
        };

        private object _providerType;
        private string _dataTypeName;
        private int? _numericPrecision;
        private int? _numericScale;
        private int? _columnSize;
        private bool? _isLong;

        public object ProviderType
        {
            get { return _providerType; }
            set { _providerType = value; }
        }

        public string DataTypeName
        {
            get { return _dataTypeName; }
            set { _dataTypeName = value; }
        }

        public int? NumericPrecision
        {
            get { return _numericPrecision; }
            set { _numericPrecision = value; }
        }

        public int? NumericScale
        {
            get { return _numericScale; }
            set { _numericScale = value; }
        }

        public int? ColumnSize
        {
            get { return _columnSize; }
            set { _columnSize = value; }
        }

        public bool? IsLong
        {
            get { return _isLong; }
            set { _isLong = value; }
        }

        public SqlDbType? SqlProviderType()
        {
            SqlDbType? provider = null;

            if (ProviderType != null && ProviderType is int)
            {
                provider = new Nullable<SqlDbType>((SqlDbType) ProviderType);
            }

            return provider;
        }

        public string ColumnDeclaration()
        {
            SqlDbType? provider = SqlProviderType();

            if (provider.HasValue)
            {
                switch (provider.Value)
                {
                    case SqlDbType.BigInt:
                        return "bigint";
                    case SqlDbType.Binary:
                        return string.Format(CultureInfo.InvariantCulture, "binary({0})", (ColumnSize ?? 1));
                    case SqlDbType.Bit:
                        return "bit";
                    case SqlDbType.Char:
                        return string.Format(CultureInfo.InvariantCulture, "char({0})", (ColumnSize ?? 1));
                    case SqlDbType.DateTime:
                        return "datetime";
                    case SqlDbType.Decimal:
                        return string.Format(CultureInfo.InvariantCulture, "decimal({0},{1})", (NumericPrecision ?? 18), (NumericScale ?? 0));
                    case SqlDbType.Float:
                        return string.Format(CultureInfo.InvariantCulture, "float({0})", (NumericPrecision ?? 53));
                    case SqlDbType.Image:
                        return string.Format(CultureInfo.InvariantCulture, "image({0})", (NumericPrecision ?? 8000));
                    case SqlDbType.Int:
                        return "int";
                    case SqlDbType.Money:
                        return "money";
                    case SqlDbType.NChar:
                        return string.Format(CultureInfo.InvariantCulture, "nchar({0})", (ColumnSize ?? 1));
                    case SqlDbType.NText:
                        return string.Format(CultureInfo.InvariantCulture, "ntext({0})", (ColumnSize ?? 1));
                    case SqlDbType.NVarChar:
                        return string.Format(CultureInfo.InvariantCulture, "nvarchar({0})", (ColumnSize ?? 1));
                    case SqlDbType.Real:
                        return "real";
                    case SqlDbType.SmallDateTime:
                        return "smalldatetime";
                    case SqlDbType.SmallInt:
                        return "smallint";
                    case SqlDbType.SmallMoney:
                        return "smallmoney";
                    case SqlDbType.Text:
                        return string.Format(CultureInfo.InvariantCulture, "text({0})", (ColumnSize ?? 1));
                    case SqlDbType.Timestamp:
                        return "timestamp";
                    case SqlDbType.TinyInt:
                        return "tinyint";
                    case SqlDbType.UniqueIdentifier:
                        return "uniqueidentifier";
                    case SqlDbType.VarBinary:
                        return string.Format(CultureInfo.InvariantCulture, "varbinary({0})", (ColumnSize ?? 1));
                    case SqlDbType.VarChar:
                        return string.Format(CultureInfo.InvariantCulture, "varchar({0})", (ColumnSize ?? 1));
                    case SqlDbType.Xml:
                        return "xml";
                    default:
                        return string.Empty;
                }
            }
            else if (!string.IsNullOrEmpty((DataTypeName)))
            {
                return DataTypeName;
            }
            else
            {
                return string.Empty;
            }
        }

        public static bool operator !=(DataColumnInfo dataColumnInfo1, DataColumnInfo dataColumnInfo2)
        {
            return !dataColumnInfo1.Equals(dataColumnInfo2);
        }

        public static bool operator ==(DataColumnInfo dataColumnInfo1, DataColumnInfo dataColumnInfo2)
        {
            return dataColumnInfo1.Equals(dataColumnInfo2);
        }

        public bool Equals(DataColumnInfo dataColumnInfo)
        {
            if (!Equals(ProviderType, dataColumnInfo.ProviderType)) return false;
            if (!Equals(DataTypeName, dataColumnInfo.DataTypeName)) return false;
            if (!Equals(NumericPrecision, dataColumnInfo.NumericPrecision)) return false;
            if (!Equals(NumericScale, dataColumnInfo.NumericScale)) return false;
            if (!Equals(ColumnSize, dataColumnInfo.ColumnSize)) return false;
            if (!Equals(IsLong, dataColumnInfo.IsLong)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is DataColumnInfo)) return false;
            return Equals((DataColumnInfo) obj);
        }

        public override int GetHashCode()
        {
            int result = ProviderType != null ? ProviderType.GetHashCode() : 0;
            result = 29*result + (DataTypeName != null ? DataTypeName.GetHashCode() : 0);
            result = 29*result + NumericPrecision.GetHashCode();
            result = 29*result + NumericScale.GetHashCode();
            result = 29*result + ColumnSize.GetHashCode();
            result = 29*result + IsLong.GetHashCode();
            return result;
        }

        public static DataColumnInfo FromSchemaRow(DataRow row)
        {
            DataColumnInfo info = new DataColumnInfo();

            for (int i = 0; i < ExtendedProperties.Length; i++)
            {
                try
                {
                    object value = row[ExtendedProperties[i]];

                    if (value == null)
                        continue;

                    switch (i)
                    {
                        case 0:
                            info.ProviderType = value;
                            break;
                        case 1:
                            info.DataTypeName = (string) value;
                            break;
                        case 2:
                            info.NumericPrecision = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                            break;
                        case 3:
                            info.NumericScale = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                            break;
                        case 4:
                            info.ColumnSize = Convert.ToInt32(value, CultureInfo.InvariantCulture);
                            break;
                        case 5:
                            info.IsLong = (bool) value;
                            break;
                    }
                }
                catch (ArgumentException e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            return info;
        }

        public static DataColumnInfo FromTypeCode(TypeCode code)
        {
            DataColumnInfo info = new DataColumnInfo();

            switch (code)
            {
                case TypeCode.Boolean:
                    info.DataTypeName = "bit";
                    info.ProviderType = SqlDbType.Bit;
                    break;
                case TypeCode.Byte:
                    info.DataTypeName = "tinyint";
                    info.ProviderType = SqlDbType.TinyInt;
                    break;
                case TypeCode.Char:
                    info.DataTypeName = "char";
                    break;
                case TypeCode.DateTime:
                    info.DataTypeName = "datetime";
                    info.ProviderType = SqlDbType.DateTime;
                    break;
                case TypeCode.Decimal:
                    info.DataTypeName = "decimal";
                    info.ProviderType = SqlDbType.Decimal;
                    break;
                case TypeCode.Double:
                    info.DataTypeName = "float";
                    info.ProviderType = SqlDbType.Float;
                    break;
                case TypeCode.Int16:
                    info.DataTypeName = "smallint";
                    info.ProviderType = SqlDbType.SmallInt;
                    break;
                case TypeCode.Int32:
                    info.DataTypeName = "int";
                    info.ProviderType = SqlDbType.Int;
                    break;
                case TypeCode.Int64:
                    info.DataTypeName = "bigint";
                    info.ProviderType = SqlDbType.BigInt;
                    break;
                case TypeCode.SByte:
                    info.DataTypeName = "smallint";
                    info.ProviderType = SqlDbType.SmallInt;
                    break;
                case TypeCode.Single:
                    info.DataTypeName = "float";
                    info.ProviderType = SqlDbType.Float;
                    break;
                case TypeCode.String:
                    info.DataTypeName = "varchar";
                    break;
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                    info.DataTypeName = "int";
                    info.ProviderType = SqlDbType.Int;
                    break;
                case TypeCode.UInt64:
                    info.DataTypeName = "bigint";
                    info.ProviderType = SqlDbType.BigInt;
                    break;
            }

            return info;
        }
    }
}
