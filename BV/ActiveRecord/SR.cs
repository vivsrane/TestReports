using System.Globalization;
using System.Resources;
using System.Threading;

namespace VB.Common.ActiveRecord
{
    internal sealed class SR
    {
        #region Resource Keys
        
        internal const string RowDataGateway_Missing_ColumnAttribute = "RowDataGateway_Missing_ColumnAttribute";

        internal const string RowDataGateway_Incomplete_IdAttribute = "RowDataGateway_Incomplete_IdAttribute";

        internal const string RowDataGateway_Missing_IdAttribute = "RowDataGateway_Missing_IdAttribute";

        internal const string RowDataGateway_Missing_TableAttribute = "RowDataGateway_Missing_TableAttribute";

        internal const string RowDataGateway_Bad_Update_Count = "RowDataGateway_Bad_Update_Count";

        internal const string RowDataGateway_Null_Column = "RowDataGateway_Null_Column";

        internal const string RowDataGateway_Null_Property = "RowDataGateway_Null_Property";

        internal const string RowDataGateway_Duplicate_IdAttribute = "RowDataGateway_Duplicate_IdAttribute";

        internal const string Finder_Bad_Row_Count = "Finder_Bad_Row_Count";

        internal const string ManyToMany_Db_Remove_NonMember = "ManyToMany_Db_Remove_NonMember";

        internal const string ManyToMany_Collection_Remove_NonMember = "ManyToMany_Collection_Remove_NonMember";

        internal const string ManyToMany_Bad_Delete_Count = "ManyToMany_Bad_Delete_Count";

        #endregion

        private readonly ResourceManager resources;
        
        internal SR()
        {
            resources = new ResourceManager("VB.Common.ActiveRecord.ActiveRecord", GetType().Assembly);
        }

        private static SR _loader;

        private static object _InternalSyncObject;

        private static SR GetLoader()
        {
            if (_loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (_loader == null)
                    {
                        _loader = new SR();
                    }
                }
            }
            return _loader;
        }

        public static string GetString(string name)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            return loader.resources.GetString(name, CultureInfo.CurrentUICulture);
        }

        public static string GetString(string name, params object[] args)
        {
            SR loader = GetLoader();
            if (loader == null)
            {
                return null;
            }
            string format = loader.resources.GetString(name, CultureInfo.CurrentUICulture);
            if ((args == null) || (args.Length <= 0))
            {
                return format;
            }
            for (int i = 0; i < args.Length; i++)
            {
                string str2 = args[i] as string;
                if ((str2 != null) && (str2.Length > 0x400))
                {
                    args[i] = str2.Substring(0, 0x3fd) + "...";
                }
            }
            return string.Format(CultureInfo.CurrentCulture, format, args);
        }

        private static object InternalSyncObject
        {
            get
            {
                if (_InternalSyncObject == null)
                {
                    object obj2 = new object();
                    Interlocked.CompareExchange(ref _InternalSyncObject, obj2, null);
                }
                return _InternalSyncObject;
            }
        }
    }
}
