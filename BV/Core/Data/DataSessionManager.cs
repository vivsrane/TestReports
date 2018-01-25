using System;
using System.Collections;
using System.Data;
using System.Threading;

namespace VB.Common.Core.Data
{
    public class DataSessionManager : IDataSessionManager
    {
        private readonly LocalDataStoreSlot _sessionSlot = Thread.GetNamedDataSlot("_sessionSlot");

        public IDataSession CreateSession(string databaseName)
        {
            DataSession session = Thread.GetData(_sessionSlot) as DataSession;

            if (session != null)
            {
                return new SessionWrapper(session);
            }

            session = new DataSession(databaseName);

            session.Closed += OnSessionClosed;

            Thread.SetData(_sessionSlot, session);

            return session;
        }

        public IDataSession Session
        {
            get
            {
                DataSession session = Thread.GetData(_sessionSlot) as DataSession;

                if (session == null)
                {
                    throw new InvalidOperationException("No open session");
                }

                return new SessionWrapper(session);
            }
        }

        void OnSessionClosed(object sender, EventArgs e)
        {
            DataSession session = sender as DataSession;

            if (session != null)
            {
                session.Closed -= OnSessionClosed;
            }
            
            Thread.SetData(_sessionSlot, null);
        }

        /// <summary>
        /// A class has no rights to close a session it did not open.  This functionality is used
        /// to enlist in an existing session (and its transactions) so functionality can be
        /// composed into larger units of work.
        /// </summary>
        class SessionWrapper : Closable, IDataSession
        {
            private IDataSession _session;
            
            public SessionWrapper(IDataSession session)
            {
                _session = session;
            }

            public IDataConnection OpenConnection()
            {
                return new ConnectionWrapper(_session.OpenConnection());
            }

            public IDataConnection Connection
            {
                get { return new ConnectionWrapper(_session.Connection); }
            }

            public IDictionary Items
            {
                get
                {
                    EnsureOpen();

                    return _session.Items;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _session = null;
                }
            }
        }

        class ConnectionWrapper : Closable, IDataConnection
        {
            private IDataConnection _connection;

            public ConnectionWrapper(IDataConnection connection)
            {
                _connection = connection;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _connection = null;
                }
            }

            public ITransaction BeginTransaction()
            {
                EnsureOpen();

                _connection.BeginTransaction();

                return new NullTransaction();
            }

            public IDbCommand CreateCommand()
            {
                EnsureOpen();

                return _connection.CreateCommand();
            }
        }

        class NullTransaction : Closable, ITransaction
        {
            protected override void Dispose(bool disposing)
            {
            }

            public void Commit()
            {
                EnsureOpen();
            }

            public void Rollback()
            {
                EnsureOpen();
            }
        }
    }
}