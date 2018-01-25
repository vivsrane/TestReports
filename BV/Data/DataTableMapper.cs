using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using VB.Common.Core.Utilities;

namespace VB.Common.Data
{
    public static class DataTableMapper
    {
        public static DataTable ToDataTable(IDataReader reader, IDataMap dataMap, DataTableCallback callback)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            DataTable dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            List<string> columnNames = new List<string>();
            List<bool> nullables = new List<bool>();
            List<Type> types = new List<Type>();

            AddPrimaryKeyColumn(dataTable);

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow schemaTableRow = schemaTable.Rows[i];

                if (!dataTable.Columns.Contains(schemaTableRow["ColumnName"].ToString()))
                {
                    string columnName = schemaTableRow["ColumnName"].ToString();
                    IDataMapEntry entry = dataMap.FindColumn(columnName);
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.ColumnName = entry.FieldName;
                    dataColumn.Unique = Convert.ToBoolean(schemaTableRow["IsUnique"], CultureInfo.InvariantCulture);
                    dataColumn.AllowDBNull = Convert.ToBoolean(schemaTableRow["AllowDBNull"], CultureInfo.InvariantCulture);
                    dataColumn.ReadOnly = Convert.ToBoolean(schemaTableRow["IsReadOnly"], CultureInfo.InvariantCulture);
                    dataColumn.DataType = (entry.TypeName == null)
                                              ? (Type) schemaTableRow["DataType"]
                                              : Type.GetType(entry.TypeName);
                    dataColumn.ExtendedProperties.Add("DataColumnInfo", DataColumnInfo.FromSchemaRow(schemaTableRow));
                    columnNames.Add(columnName);
                    types.Add(dataColumn.DataType);
                    nullables.Add(dataColumn.AllowDBNull);
                    dataTable.Columns.Add(dataColumn);
                }
            }

            foreach (DataColumn column in callback.NewColumns)
                dataTable.Columns.Add(column);

            while (reader.Read())
            {
                DataRow dataRow = dataTable.NewRow();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    string name = dataMap.FindColumn(columnNames[i]).FieldName;
                    object value = reader[columnNames[i]];
                    dataRow[name] = ConversionHelper.Convert(value, types[i], ConversionHelper.DefaultValue(types[i], nullables[i]));
                }

                if ( callback.RegisterRow(dataRow) )
                {
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }

        public static DataTable ToDataTable(IDataReader reader)
        {
            return ToDataTable(reader, new IdentityDataMap(reader.GetSchemaTable()), new DataTableCallback());
        }
        public static DataTable ToOlapDataTable(IDataReader reader, IDataMap dataMap, DataTableCallback callback)
        {
            DataTable schemaTable = reader.GetSchemaTable();
            DataTable dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;
            List<string> columnNames = new List<string>();
            List<bool> nullables = new List<bool>();
            List<Type> types = new List<Type>();

            AddPrimaryKeyColumn(dataTable);

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow schemaTableRow = schemaTable.Rows[i];

                if (!dataTable.Columns.Contains(schemaTableRow["ColumnName"].ToString()))
                {
                    string columnName = schemaTableRow["ColumnName"].ToString();
                    IDataMapEntry entry = dataMap.FindOlapColumn(columnName);
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.ColumnName = entry.FieldName;
                    dataColumn.Unique = Convert.ToBoolean(schemaTableRow["IsUnique"], CultureInfo.InvariantCulture);
                    dataColumn.AllowDBNull = Convert.ToBoolean(schemaTableRow["AllowDBNull"], CultureInfo.InvariantCulture);
                    dataColumn.ReadOnly = Convert.ToBoolean(schemaTableRow["IsReadOnly"], CultureInfo.InvariantCulture);
                    dataColumn.DataType = (entry.TypeName == null)
                                              ? (Type)schemaTableRow["DataType"]
                                              : Type.GetType(entry.TypeName);
                    dataColumn.ExtendedProperties.Add("DataColumnInfo", DataColumnInfo.FromSchemaRow(schemaTableRow));
                    columnNames.Add(columnName);
                    types.Add(dataColumn.DataType);
                    nullables.Add(dataColumn.AllowDBNull);
                    dataTable.Columns.Add(dataColumn);
                }
            }

            foreach (DataColumn column in callback.NewColumns)
                dataTable.Columns.Add(column);

            while (reader.Read())
            {
                DataRow dataRow = dataTable.NewRow();

                for (int i = 0; i < columnNames.Count; i++)
                {
                    string name = dataMap.FindOlapColumn(columnNames[i]).FieldName;
                    object value = reader[columnNames[i]];
                    dataRow[name] = ConversionHelper.Convert(value, types[i], ConversionHelper.DefaultValue(types[i], nullables[i]));
                }

                if (callback.RegisterRow(dataRow))
                {
                    dataTable.Rows.Add(dataRow);
                }
            }

            return dataTable;
        }
        internal static void AddPrimaryKeyColumn(DataTable dataTable)
        {
            DataColumn primaryKey = new DataColumn("__RowID__", Type.GetType("System.Int32"));
            primaryKey.AutoIncrement = true;
            primaryKey.AutoIncrementSeed = 1;
            primaryKey.AutoIncrementStep = 1;
            dataTable.Columns.Add(primaryKey);
            dataTable.PrimaryKey = new DataColumn[] { primaryKey };
        }
    }
}
