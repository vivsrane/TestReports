using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.ActiveRecord
{
    [Serializable]
    public class Finder<T> where T : class, new()
    {
        public T FindFirst(IDictionary<string, object> whereClause)
        {
            return First(Find(whereClause));
        }

        public IList<T> FindAll()
        {
            return Find(new Dictionary<string, object>());
        }

        public IList<T> Find(IDictionary<string, object> whereClause)
        {
            return GetRowDataGateway().Select(WhereClauseToBindingList(whereClause));
        }

        public IList<T> Find(string commandText, IList<RowDataGatewayBinding> bindings)
        {
            return GetRowDataGateway().Select(commandText, bindings);
        }

        public T First(IList<T> list)
        {
            T item;

            switch (list.Count)
            {
                case 0:
                    item = null;
                    break;
                case 1:
                    item = list[0];
                    break;
                default:
                    throw new ArgumentException(SR.GetString(SR.Finder_Bad_Row_Count, new object[] {list.Count, 1}));
            }

            return item;
        }

        public IDictionary<string, object> CreateWhereClause(string column, object value)
        {
            IDictionary<string, object> whereClause = new Dictionary<string, object>();
            whereClause.Add(column, value);
            return whereClause;
        }

        protected static IList<RowDataGatewayBinding> CreateBindingList(string name, object value, DbType type, bool nullable)
        {
            IList<RowDataGatewayBinding> bindings = new List<RowDataGatewayBinding>();
            bindings.Add(new RowDataGatewayBinding(name, value, type, nullable));
            return bindings;
        }

        protected IList<RowDataGatewayBinding> WhereClauseToBindingList(IDictionary<string, object> whereClause)
        {
            IList<RowDataGatewayBinding> bindings = new List<RowDataGatewayBinding>();

            foreach (string column in whereClause.Keys)
            {
                bindings.Add(GetRowDataGateway().Where(column, whereClause[column]));
            }

            return bindings;
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "The method performs a time-consuming operation.")]
        protected RowDataGateway<T> GetRowDataGateway()
        {
            return RowDataGatewayRegistry<T>.GetRowDataGateway();
        }
    }
}
