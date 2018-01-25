using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using VB.Common.Data;

namespace VB.Common.ActiveRecord
{
    /// <summary>
    /// Defines a many-valued association with many-to-many multiplicity.
    /// </summary>
    /// <typeparam name="TMappedBy">The type that owns the relationship.</typeparam>
    /// <typeparam name="TTargetEntity">The type that is the target of the association.</typeparam>
    [Serializable]
    public class ManyToManyCollection<TMappedBy, TTargetEntity> : Finder<TTargetEntity>, ICollection<TTargetEntity>
        where TMappedBy : class, new()
        where TTargetEntity : class, new()
    {
        private readonly string tableName;
        private readonly TMappedBy mappedBy;
        private ICollection<TTargetEntity> values;
        private bool select;

        private readonly string addSql;
        private readonly IList<IDataParameterTemplate> addParameterTemplates;

        private readonly string clearSql;
        private readonly IList<IDataParameterTemplate> clearParameterTemplates;

        private readonly string removeSql;
        private readonly IList<IDataParameterTemplate> removeParameterTemplates;

        public ManyToManyCollection(TMappedBy mappedBy, string tableName)
        {
            this.tableName = tableName;
            this.mappedBy = mappedBy;
            values = new List<TTargetEntity>();
            select = true;
            GenerateAddSql(out addSql, out addParameterTemplates);
            GenerateClearSql(out clearSql, out clearParameterTemplates);
            GenerateRemoveSql(out removeSql, out removeParameterTemplates);
        }

        #region ICollection<TTargetEntity> Members

        public virtual void Add(TTargetEntity item)
        {
            if (Contains(item))
                return;
            // parameter bindings
            IDictionary<string,object> parameters = new Dictionary<string,object>();
            parameters.Add("@JoinColumn", RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKey.Get(mappedBy));
            parameters.Add("@InverseJoinColumn", RowDataGatewayRegistry<TTargetEntity>.GetRowDataGateway().PrimaryKey.Get(item));
            // run command
            SimpleQuery.ExecuteNonQuery(RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().DatabaseName, CommandType.Text, addSql, addParameterTemplates, parameters, false);
            // add to collection
            values.Add(item);
        }

        public virtual void Clear()
        {
            // parameter bindings
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@JoinColumn", RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKey.Get(mappedBy));
            // run command
            SimpleQuery.ExecuteNonQuery(RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().DatabaseName, CommandType.Text, clearSql, clearParameterTemplates, parameters, false);
            // update local collection
            values.Clear();
        }

        public bool Contains(TTargetEntity item)
        {
            return Values().Contains(item);
        }

        public void CopyTo(TTargetEntity[] array, int arrayIndex)
        {
            Values().CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return Values().Count; }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public virtual bool Remove(TTargetEntity item)
        {
            // parameter bindings
            IDictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("@JoinColumn", RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKey.Get(mappedBy));
            parameters.Add("@InverseJoinColumn", RowDataGatewayRegistry<TTargetEntity>.GetRowDataGateway().PrimaryKey.Get(item));
            // run command
            NonQueryResult result = SimpleQuery.ExecuteNonQuery(RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().DatabaseName, CommandType.Text, removeSql, removeParameterTemplates, parameters, false);
            // remove from collection
            bool removedItem = values.Remove(item);
            if (result.RowsAffected == 1)
            {
                if (removedItem)
                    return true;
                throw new DataException(SR.GetString(SR.ManyToMany_Db_Remove_NonMember));
            }
            else if (result.RowsAffected == 0)
            {
                if (!removedItem)
                    return false;
                throw new DataException(SR.GetString(SR.ManyToMany_Collection_Remove_NonMember));
            }
            else
            {
                throw new DataException(SR.GetString(SR.ManyToMany_Bad_Delete_Count, new object[] {result.RowsAffected, removedItem ? 1 : 0}));
            }
        }

        #endregion

        #region IEnumerable<TTargetEntity> Members

        public IEnumerator<TTargetEntity> GetEnumerator()
        {
            return Values().GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (Values() as System.Collections.IEnumerable).GetEnumerator();
        }

        #endregion

        private ICollection<TTargetEntity> Values()
        {
            if (select)
            {
                object value = RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKey.Get(mappedBy);

                if (value == null)
                {
                    values = new List<TTargetEntity>();
                }
                else
                {
                    DbType type = GetRowDataGateway().PrimaryKeyColumn.DbType;

                    values = Find(JoinStatement(), CreateBindingList("Value", value, type, false));
                }

                select = false;
            }

            return values;
        }

        protected virtual string JoinStatement()
        {
            ColumnAttribute joinColumn = RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKeyColumn;
            ColumnAttribute inverseJoinColumn = RowDataGatewayRegistry<TTargetEntity>.GetRowDataGateway().PrimaryKeyColumn;
            return GetRowDataGateway().JoinStatement(tableName, inverseJoinColumn.Name, joinColumn.Name);
        }

        private void GenerateRemoveSql(out string text, out IList<IDataParameterTemplate> parameters)
        {
            ColumnAttribute joinColumn = RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKeyColumn;
            ColumnAttribute inverseJoinColumn = RowDataGatewayRegistry<TTargetEntity>.GetRowDataGateway().PrimaryKeyColumn;
            // command text
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM ").Append(tableName).Append(" WHERE ");
            sb.Append(joinColumn.Name).Append(" = @JoinColumn");
            sb.Append(" AND ").Append(inverseJoinColumn.Name).Append(" = @InverseJoinColumn");
            text = sb.ToString();
            // parameter definitions
            parameters = new List<IDataParameterTemplate>();
            parameters.Add(new DataParameterTemplate("@JoinColumn", joinColumn.DbType, joinColumn.Nullable));
            parameters.Add(new DataParameterTemplate("@InverseJoinColumn", inverseJoinColumn.DbType, inverseJoinColumn.Nullable));
        }

        private void GenerateClearSql(out string text, out IList<IDataParameterTemplate> parameters)
        {
            ColumnAttribute joinColumn = RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKeyColumn;
            // command text
            StringBuilder sb = new StringBuilder();
            sb.Append("DELETE FROM ").Append(tableName).Append(" WHERE ");
            sb.Append(joinColumn.Name).Append(" = @JoinColumn");
            text = sb.ToString();
            // parameter definitions
            parameters = new List<IDataParameterTemplate>();
            parameters.Add(new DataParameterTemplate("@JoinColumn", joinColumn.DbType, joinColumn.Nullable));
        }

        private void GenerateAddSql(out string text, out IList<IDataParameterTemplate> parameters)
        {
            ColumnAttribute joinColumn = RowDataGatewayRegistry<TMappedBy>.GetRowDataGateway().PrimaryKeyColumn;
            ColumnAttribute inverseJoinColumn = RowDataGatewayRegistry<TTargetEntity>.GetRowDataGateway().PrimaryKeyColumn;
            // command text
            StringBuilder sb = new StringBuilder();
            sb.Append("INSERT INTO ").Append(tableName);
            sb.Append("(").Append(joinColumn.Name).Append(",").Append(inverseJoinColumn.Name).Append(")");
            sb.Append(" VALUES (").Append("@JoinColumn").Append(",").Append("@InverseJoinColumn").Append(")");
            text = sb.ToString();
            // parameter definitions
            parameters = new List<IDataParameterTemplate>();
            parameters.Add(new DataParameterTemplate("@JoinColumn", joinColumn.DbType, joinColumn.Nullable));
            parameters.Add(new DataParameterTemplate("@InverseJoinColumn", inverseJoinColumn.DbType, inverseJoinColumn.Nullable));
        }
    }
}
