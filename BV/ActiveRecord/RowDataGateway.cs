using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;
using VB.Common.Core.Utilities;

namespace VB.Common.ActiveRecord
{
    public sealed class RowDataGateway<T> where T : class, new()
    {
        private readonly string tableName;
        private readonly string databaseName;
        private readonly ColumnAttribute primaryKeyColumn;
        private readonly DynamicMember primaryKeyProperty;
        private readonly ColumnAttribute[] columns;
        private readonly DynamicMember[] properties;

        private readonly string insertSQL;
        private readonly string updateSQL;

        public string TableName
        {
            get { return tableName; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
        }

        public DynamicMember PrimaryKey
        {
            get { return primaryKeyProperty; }
        }

        public ColumnAttribute PrimaryKeyColumn
        {
            get { return primaryKeyColumn; }
        }

        public DynamicMember Member(string memberName)
        {
            foreach (DynamicMember property in properties)
            {
                if (property.MemberInfo.Name.Equals(memberName))
                {
                    return property;
                }
            }

            return null;
        }

        public ColumnAttribute Column(string memberName)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                DynamicMember property = properties[i];

                if (property.MemberInfo.Name.Equals(memberName))
                {
                    return columns[i];
                }
            }
            
            return null;
        }

        public RowDataGateway()
        {
            TableInformation(out databaseName, out tableName);
            ColumnInformation(out primaryKeyColumn, out primaryKeyProperty, out columns, out properties);

            insertSQL = GenerateInsertSQL();
            updateSQL = GenerateUpdateSQL();
        }

        public string ToString(T value)
        {
            StringBuilder sb = new StringBuilder("[");
            sb.Append(primaryKeyProperty.MemberInfo.Name).Append("=").Append(primaryKeyProperty.Get(value));
            for (int i = 0; i < properties.Length; i++)
            {
                sb.Append(",").Append(properties[i].MemberInfo.Name).Append("=").Append(properties[i].Get(value));
            }
            return sb.Append("]").ToString();
        }

        public int HashCode(T value)
        {
            int hashCode = 1;
            foreach (DynamicMember member in properties)
            {
                hashCode = 31 + hashCode + (member.Get(value) == null ? 0 : member.Get(value).GetHashCode());
            }
            return hashCode;
        }

        public RowDataGatewayBinding Where(string propertyName, object value)
        {
            if (primaryKeyProperty.MemberInfo.Name.Equals(propertyName))
                return new RowDataGatewayBinding(primaryKeyColumn, primaryKeyProperty, value);

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].MemberInfo.Name.Equals(propertyName))
                {
                    return new RowDataGatewayBinding(columns[i], properties[i], value);
                }
            }

            throw new NoColumnAttributeException(SR.GetString(SR.RowDataGateway_Missing_ColumnAttribute, new object[] { propertyName, typeof(T).FullName }));
        }

        public IList<T> Select(IList<RowDataGatewayBinding> bindings)
        {
            return Select(SelectStatement(bindings), bindings);
        }

        public IList<T> Select(string commandText, IList<RowDataGatewayBinding> bindings)
        {
            return ExecuteReader(bindings, commandText);
        }

        public void Save(T item)
        {
            if (primaryKeyProperty.Get(item) == null)
            {
                Insert(item);
            }
            else
            {
                Update(item);
            }
        }

        public void Insert(T item)
        {
            Execute(item, insertSQL, false);
        }

        public void Update(T item)
        {
            Execute(item, updateSQL, true);
        }

        private void Execute(T item, string commandText, bool bindPrimaryKey)
        {
            ActiveConnection activeConnection = new ActiveConnection(DatabaseName);

            using (DbConnection connection = activeConnection.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    using (DbTransaction transaction = connection.BeginTransaction())
                    {
                        command.Connection = connection;
                        command.Transaction = transaction;
                        command.CommandText = commandText;

                        Bind(item, command);

                        if (bindPrimaryKey) BindPrimaryKey(item, command);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (rowsAffected != 1)
                        {
                            throw new ActiveRecordException(SR.GetString(SR.RowDataGateway_Bad_Update_Count, new object[] {rowsAffected, 1}));
                        }

                        if (!bindPrimaryKey)
                        {
                            using (DbCommand primaryKeyCommand = connection.CreateCommand())
                            {
                                primaryKeyCommand.Connection = connection;
                                primaryKeyCommand.Transaction = transaction;
                                primaryKeyCommand.CommandText = "SELECT @@IDENTITY Id";

                                Type primaryKeyType = ((PropertyInfo) primaryKeyProperty.MemberInfo).PropertyType;

                                object primaryKeyRawValue = primaryKeyCommand.ExecuteScalar();

                                object primaryKeyValue = ConversionHelper.Convert(primaryKeyRawValue, primaryKeyType);

                                primaryKeyProperty.Set(item, primaryKeyValue);
                            }
                        }

                        transaction.Commit();
                    }
                }
            }
        }

        private IList<T> ExecuteReader(IEnumerable<RowDataGatewayBinding> bindings, string commandText)
        {
            ActiveConnection activeConnection = new ActiveConnection(DatabaseName);

            using (DbConnection connection = activeConnection.CreateConnection())
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (DbCommand command = connection.CreateCommand())
                {
                    command.Connection = connection;
                    command.CommandText = commandText;

                    Bind(bindings, command);

                    using (DbDataReader reader = command.ExecuteReader())
                    {
                        List<T> items = new List<T>();

                        while (reader.Read())
                        {
                            items.Add(Marshall(reader));
                        }

                        return items;
                    }
                }
            }
        }

        private static void Bind(IEnumerable<RowDataGatewayBinding> bindings, DbCommand command)
        {
            foreach (RowDataGatewayBinding binding in bindings)
            {
                BindColumn(command, binding.Column, binding.Value);
            }
        }

        private void Bind(T item, DbCommand command)
        {
            for (int i = 0; i < columns.Length; i++)
            {
                BindColumn(item, command, columns[i], properties[i]);
            }
        }

        private void BindPrimaryKey(T item, DbCommand command)
        {
            BindColumn(item, command, primaryKeyColumn, primaryKeyProperty);
        }

        private static void BindColumn(T item, DbCommand command, ColumnAttribute column, DynamicMember property)
        {
            BindColumn(command, column, property.Get(item));
        }

        private static void BindColumn(DbCommand command, ColumnAttribute column, object value)
        {
            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@" + column.Name;
            parameter.DbType = column.DbType;
            if (value == null)
            {
                if (column.Nullable)
                {
                    parameter.Value = DBNull.Value;
                    parameter.SourceColumnNullMapping = true;
                }
                else
                {
                    throw new ArgumentNullException(SR.GetString(SR.RowDataGateway_Null_Column, new object[] { column.Name }));
                }
            }
            else
            {
                parameter.Value = value;
                parameter.SourceColumnNullMapping = column.Nullable;
            }
            command.Parameters.Add(parameter);
        }

        private T Marshall(IDataRecord reader)
        {
            T item = new T();

            MarshallProperty(item, reader, primaryKeyColumn, primaryKeyProperty);

            for (int i = 0; i < columns.Length; i++)
            {
                MarshallProperty(item, reader, columns[i], properties[i]);
            }

            return item;
        }

        private static void MarshallProperty(T item, IDataRecord reader, ColumnAttribute column, DynamicMember property)
        {
            if (reader[column.Name].Equals(DBNull.Value))
            {
                if (column.Nullable)
                {
                    property.Set(item, null);
                }
                else
                {
                    throw new ArgumentNullException(SR.GetString(SR.RowDataGateway_Null_Property, new object[] {property.MemberInfo.Name}));
                }
            }
            else
            {
                property.Set(item, reader[column.Name]);
            }
        }

        private string SelectStatement(IList<RowDataGatewayBinding> bindings)
        {
            StringBuilder stmt = new StringBuilder();

            stmt.Append("SELECT ").Append(ColumnList(null)).Append(" FROM ").Append(tableName);

            for (int i = 0; i < bindings.Count; i++)
            {
                ColumnAttribute column = bindings[i].Column;

                if (i == 0)
                {
                    stmt.Append(" WHERE ");
                }
                else
                {
                    stmt.Append(" AND ");
                }

                if (!string.IsNullOrEmpty(column.SelectCast))
                {
                    stmt.Append(column.SelectCast).Append(" = @").Append(column.Name);
                }
                else
                {
                    stmt.Append(column.Name).Append(" = @").Append(column.Name);
                }
            }
            return stmt.ToString();
        }

        public string JoinStatement(string joinTable, string inverseJoinColumn, string joinColumn)
        {
            StringBuilder stmt = new StringBuilder();

            stmt.Append("SELECT ").Append(ColumnList("T")).Append(" FROM ").Append(tableName).Append(" T ");
            
            stmt.Append(" JOIN ").Append(joinTable).Append(" J ON T.");

            if (!string.IsNullOrEmpty(primaryKeyColumn.SelectCast))
            {
                stmt.Append(primaryKeyColumn.SelectCast.Replace(primaryKeyColumn.Name, "T." + primaryKeyColumn.Name));
            }
            else
            {
                stmt.Append(primaryKeyColumn.Name);
            }

            stmt.Append(" = J.").Append(inverseJoinColumn);
            
            stmt.Append(" WHERE J.").Append(joinColumn).Append(" = @Value");

            return stmt.ToString();
        }

        public string ColumnList(string alias)
        {
            StringBuilder stmt = new StringBuilder();

            stmt.Append(ColumnItem(primaryKeyColumn, alias, false));

            for (int i = 0; i < columns.Length; i++)
            {
                stmt.Append(ColumnItem(columns[i], alias, true));
            }

            return stmt.ToString();
        }

        private static string ColumnItem(ColumnAttribute column, string alias, bool comma)
        {
            StringBuilder stmt = new StringBuilder();

            if (comma) stmt.Append(", ");

            if (!string.IsNullOrEmpty(column.SelectCast))
            {
                if (string.IsNullOrEmpty(alias))
                {
                    stmt.Append(column.SelectCast).Append(" ");
                }
                else
                {
                    stmt.Append(column.SelectCast.Replace(column.Name, alias + "." + column.Name)).Append(" ");
                }
            }
            else if (!string.IsNullOrEmpty(alias))
            {
                stmt.Append(alias).Append(".");
            }

            stmt.Append(column.Name);

            return stmt.ToString();
        }

        private string GenerateInsertSQL()
        {
            StringBuilder stmt = new StringBuilder();

            stmt.Append("INSERT INTO ").Append(TableName).Append(" (");

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Equals(primaryKeyColumn)) continue;

                if (i > 0) stmt.Append(", ");

                stmt.Append(columns[i].Name);
            }
            
            stmt.Append(") VALUES (");

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Equals(primaryKeyColumn)) continue;

                if (i > 0) stmt.Append(", ");

                if (!string.IsNullOrEmpty(columns[i].UpdateCast))
                    stmt.Append(columns[i].UpdateCast);
                else
                    stmt.Append("@").Append(columns[i].Name);
            }

            stmt.Append(")");

            return stmt.ToString();
        }

        private string GenerateUpdateSQL()
        {
            StringBuilder stmt = new StringBuilder();

            stmt.Append("UPDATE ").Append(TableName).Append(" SET ");

            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i].Equals(primaryKeyColumn)) continue;

                if (i > 0) stmt.Append(", ");

                stmt.Append(columns[i].Name).Append(" = ");

                if (!string.IsNullOrEmpty(columns[i].UpdateCast))
                    stmt.Append(columns[i].UpdateCast);
                else
                    stmt.Append("@").Append(columns[i].Name);
            }

            stmt.Append(" WHERE ").Append(primaryKeyColumn.Name).Append(" = ");

            if (!string.IsNullOrEmpty(primaryKeyColumn.UpdateCast))
                stmt.Append(primaryKeyColumn.UpdateCast);
            else
                stmt.Append("@").Append(primaryKeyColumn.Name);

            return stmt.ToString();
        }

        private static void TableInformation(out string databaseName, out string tableName)
        {
            Type type = typeof(T);

            foreach (object attr in type.GetCustomAttributes(typeof(TableAttribute), true))
            {
                TableAttribute table = (TableAttribute) attr;
                tableName = table.Name;
                databaseName = table.Database;
                return;
            }

            throw new MissingTableAnnotationException(SR.GetString(SR.RowDataGateway_Missing_TableAttribute, new object[] {type.FullName}));
        }

        private static void ColumnInformation(out ColumnAttribute pkColumn, out DynamicMember pkProperty, out ColumnAttribute[] columns, out DynamicMember[] properties)
        {
            pkColumn = null;
            pkProperty = null;

            Type type = typeof(T);

            List<ColumnAttribute> columnList = new List<ColumnAttribute>();
            List<DynamicMember> propertyList = new List<DynamicMember>();

            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            foreach (PropertyInfo property in type.GetProperties(flags))
            {
                foreach (object attr in property.GetCustomAttributes(typeof(ColumnAttribute), true))
                {
                    ColumnAttribute column = (ColumnAttribute)attr;

                    object[] primaryKeyAttributes = property.GetCustomAttributes(typeof(IdAttribute), true);

                    if (primaryKeyAttributes.Length > 0)
                    {
                        if (primaryKeyAttributes.Length > 1)
                            throw new ActiveRecordException(SR.GetString(SR.RowDataGateway_Duplicate_IdAttribute, new object[] {type.Name, primaryKeyAttributes.Length}));
                        pkColumn = column;
                        pkProperty = new DynamicMember(type, property);
                    }
                    else
                    {
                        propertyList.Add(new DynamicMember(type, property));
                        columnList.Add(column);
                    }
                }
            }

            foreach (FieldInfo field in type.GetFields(flags))
            {
                foreach (object attr in field.GetCustomAttributes(typeof(ColumnAttribute), true))
                {
                    propertyList.Add(new DynamicMember(type, field));
                    columnList.Add((ColumnAttribute)attr);
                }
            }

            columns = columnList.ToArray();
            properties = propertyList.ToArray();

            if (pkProperty == null)
                throw new NoPrimaryKeyAnnotationException(
                    SR.GetString(SR.RowDataGateway_Incomplete_IdAttribute,
                                 new object[] { type.Name }));

            if (pkColumn == null)
                throw new NoColumnAttributeException(
                    SR.GetString(SR.RowDataGateway_Incomplete_IdAttribute,
                                 new object[] {type.Name, pkProperty.MemberInfo.Name}));
        }
    }
}
