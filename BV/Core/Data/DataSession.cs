using System;
using System.Collections;

namespace VB.Common.Core.Data
{
    public class DataSession : Closable, IDataSession
    {
        private readonly string _databaseName;
        private IDictionary _items;

        public string DatabaseName
        {
            get { return _databaseName; }
        }

        public DataSession(string databaseName)
        {
            _databaseName = databaseName;

            _items = new Hashtable();
        }

        private DataConnection _connection;

        public IDataConnection OpenConnection()
        {
            EnsureOpen();

            if (_connection == null)
            {
                _connection = new DataConnection(_databaseName);

                _connection.Closed += Connection_Closed;
            }
            
            return _connection;
        }

        public IDataConnection Connection
        {
            get
            {
                EnsureOpen();

                if (_connection == null)
                {
                    throw new NotSupportedException("No open connection");
                }

                return _connection;
            }
        }

        public IDictionary Items
        {
            get { return _items; }
        }

        private void Connection_Closed(object sender, EventArgs e)
        {
            _connection.Closed -= Connection_Closed;

            _connection = null;
        }

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_connection != null)
                {
                    _connection.Close();

                    _connection = null;
                }

                if (_items != null)
                {
                    _items.Clear();

                    _items = null;
                }
            }
        }

        #endregion
    }
}