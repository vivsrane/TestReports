using System.Diagnostics;

namespace VB.Common.Pool.Spi
{
    internal static class Logger
    {
        private const string sourceName = "VB.Commons.Pool";

        private const string logName = "Application";

        public static void Info(string message)
        {
            Log(message, EventLogEntryType.Information);
        }

        public static void Error(string message)
        {
            Log(message, EventLogEntryType.Error);
        }

        public static void Log(string message, EventLogEntryType type)
        {
            if (!EventLog.SourceExists(sourceName))
            {
                EventLog.CreateEventSource(sourceName, logName);
            }

            EventLog log = new EventLog();

            log.Source = sourceName;

            log.WriteEntry(message, type);
        }
    }
}
