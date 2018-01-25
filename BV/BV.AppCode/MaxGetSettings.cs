using System;
using System.Data;
using VB.Common.Data;

namespace BV.AppCode
{
    /// <summary>
    /// Summary description for MaxGetSettings
    /// </summary>
    [Serializable]
    public class MaxSettings
    {
        public int BusinessUnitId { get; private set; }
        public bool HasSettings { get; private set; }
        public bool OptimalFormat { get; private set; }
        public bool PriceNewCars { get; private set; }
        public bool ShowDashboard { get; private set; }
        public bool ShowGroupLevelDashboard { get; private set; }
        public bool AnalyticsSuite { get; private set; }
        public bool BatchAutoload { get; private set; }
        public byte MaxVersion { get; private set; }
        public bool WebLoaderEnabled { get; private set; }
        public bool ModelLevelFrameworksEnabled { get; private set; }
        public bool AutoOfflineWholesalePlanTrigger { get; private set; }
        public bool ShowCtrGraph { get; private set; }

        public MaxSettings(int businessUnitId)
        {
            BusinessUnitId = businessUnitId;
            HasSettings = false;
        }

        public static MaxSettings GetSettings(int businessUnitId)
        {
            return LoadFromDatabase(businessUnitId);
        }

        private static MaxSettings LoadFromDatabase(int businessUnitId)
        {
            var settings = new MaxSettings(businessUnitId);
            settings.Run();
            return settings;
        }

        protected void Run()
        {
            using (IDataConnection connection = SimpleQuery.ConfigurationManagerConnection("Merchandising"))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "settings.MiscSettings#Fetch";
                    command.CommandType = CommandType.StoredProcedure;
                    IDbDataParameter param = command.CreateParameter();
                    param.ParameterName = "businessUnitID";
                    param.Value = BusinessUnitId;
                    param.DbType = DbType.Int32;
                    command.Parameters.Add(param);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            HasSettings = true;
                            OptimalFormat = reader.GetBoolean(reader.GetOrdinal("sendOptimalFormat"));
                            PriceNewCars = reader.GetBoolean(reader.GetOrdinal("priceNewCars"));
                            ShowDashboard = reader.GetBoolean(reader.GetOrdinal("showDashboard"));
                            ShowGroupLevelDashboard = reader.GetBoolean(reader.GetOrdinal("ShowGroupLevelDashboard"));
                            AnalyticsSuite = reader.GetBoolean(reader.GetOrdinal("analyticsSuite"));
                            BatchAutoload = reader.GetBoolean(reader.GetOrdinal("batchAutoload"));
                            MaxVersion = reader.GetByte(reader.GetOrdinal("MaxVersion"));
                            WebLoaderEnabled = reader.GetBoolean(reader.GetOrdinal("WebLoaderEnabled"));
                            ModelLevelFrameworksEnabled =
                                reader.GetBoolean(reader.GetOrdinal("ModelLevelFrameworksEnabled"));
                            AutoOfflineWholesalePlanTrigger =
                                reader.GetBoolean(reader.GetOrdinal("AutoOffline_WholesalePlanTrigger"));

                            ShowCtrGraph = reader.GetBoolean(reader.GetOrdinal("ShowCtrGraph"));
                        }
                    }
                }
            }
        }
    }
}