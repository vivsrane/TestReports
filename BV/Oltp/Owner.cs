using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Csla;
using VB.Common.Data;

namespace VB.DomainModel.Oltp
{
    [Serializable]
    public class Owner : ReadOnlyBase<Owner>
    {
        #region Business Methods

        private string _handle;
        private string _name;
        private int _id;
        private int _ownerEntityId;
        private OwnerEntityType _ownerEntityType;

        public OwnerEntityType OwnerEntityType
        {
            get { return _ownerEntityType; }
        }

        public int OwnerEntityId
        {
            get { return _ownerEntityId; }
        }

        public int Id
        {
            get { return _id; }
        }

        public string Handle
        {
            get { return _handle; }
        }

        public string Name
        {
            get { return _name; }
        }

        protected override object GetIdValue()
        {
            return _id;
        }

        #endregion

        #region Factory Methods

        private Owner()
        {
            /* force use of factory methods */
        }

        private class CachedOwner
        {
            public Owner Owner { get; set; }
            public DateTime TimeStamp { get; set; }
        }

        private static DateTime _lastCacheClean = DateTime.Now;
        private static readonly object CacheLock = new object();
        private static volatile Dictionary<string, CachedOwner> _ownerCache = new Dictionary<string, CachedOwner>();
        private static void CleanOwnerCache()
        {
            if ((DateTime.Now - _lastCacheClean).TotalHours > 2)
            {
                _ownerCache =
                    _ownerCache.Where(x => (DateTime.Now - x.Value.TimeStamp).TotalHours < 2)
                               .ToDictionary(x => x.Key, y => y.Value);
                _lastCacheClean = DateTime.Now;
            }
        }

        public static Owner GetOwner(string handle)
        {
            if (string.IsNullOrEmpty(handle))
            {
                throw new ArgumentNullException("handle");
            }
            lock (CacheLock)
            {
                CleanOwnerCache();
                if (!_ownerCache.ContainsKey(handle))
                    _ownerCache.Add(
                        handle,
                        new CachedOwner()
                        {
                            Owner = DataPortal.Fetch<Owner>(new HandleCriteria(handle)),
                            TimeStamp = DateTime.Now
                        });
            }
            return _ownerCache[handle].Owner;
        }

        public static Owner GetOwner(int dealerId)
        {
            return DataPortal.Fetch<Owner>(new IdCriteria(dealerId));
        }

        [Serializable]
        public class DataSource
        {
            public Owner Select(string handle)
            {
                return GetOwner(handle);
            }
        }

        #endregion

        #region Data Access

        protected virtual void DataPortal_Fetch(HandleCriteria criteria)
        {
            using (IDataConnection connection = SimpleQuery.ConfigurationManagerConnection(""))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                using (IDataCommand command = new DataCommand((connection.CreateCommand())))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Pricing.Owner#FetchByHandle";
                    command.AddParameterWithValue("OwnerHandle", DbType.String, false, criteria.Handle);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Fetch(reader);
                        }
                        else
                        {
                            throw new Exception("No such Owner!");
                        }
                    }
                }
            }
        }

        protected virtual void DataPortal_Fetch(IdCriteria criteria)
        {
            using (IDataConnection connection = SimpleQuery.ConfigurationManagerConnection(""))
            {
                if (connection.State == ConnectionState.Closed)
                {
                    connection.Open();
                }

                using (IDataCommand command = new DataCommand((connection.CreateCommand())))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Pricing.Owner#FetchByDealerId";
                    command.AddParameterWithValue("DealerId", DbType.Int32, false, criteria.Id);

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Fetch(reader);
                        }
                        else
                        {
                            throw new Exception("No such Owner!");
                        }
                    }
                }
            }
        }

        private void Fetch(IDataRecord record)
        {
            _id = record.GetInt32(record.GetOrdinal("Id"));
            _handle = record.GetString(record.GetOrdinal("Handle"));
            _name = record.GetString(record.GetOrdinal("Name"));
            _ownerEntityId = record.GetInt32(record.GetOrdinal("OwnerEntityId"));
            _ownerEntityType =
                (OwnerEntityType)
                Enum.ToObject(typeof (OwnerEntityType), record.GetInt32(record.GetOrdinal("OwnerEntityTypeId")));
        }

        [Serializable]
        protected class HandleCriteria
        {
            private readonly string _handle;

            public HandleCriteria(string handle)
            {
                _handle = handle;
            }

            public string Handle
            {
                get { return _handle; }
            }
        }

        [Serializable]
        protected class IdCriteria
        {
            private readonly int _id;

            public IdCriteria(int id)
            {
                _id = id;
            }

            public int Id
            {
                get { return _id; }
            }
        }

        #endregion
    }
}