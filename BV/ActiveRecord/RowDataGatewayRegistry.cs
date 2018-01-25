using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.ActiveRecord
{
    public static class RowDataGatewayRegistry<T> where T : class, new()
    {
        private static readonly IDictionary<Type, RowDataGateway<T>> registry = new Dictionary<Type, RowDataGateway<T>>();

        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        public static RowDataGateway<T> GetRowDataGateway()
        {
            RowDataGateway<T> gateway;

            lock (registry)
            {
                if (registry.Keys.Contains(typeof(T)))
                {
                    gateway = registry[typeof(T)];
                }
                else
                {
                    gateway = new RowDataGateway<T>();

                    registry.Add(typeof(T), gateway);
                }
            }

            return gateway;
        }
    }
}
