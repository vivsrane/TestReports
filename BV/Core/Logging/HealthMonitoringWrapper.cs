using System;
using System.Diagnostics;
using System.Web.Management;

namespace VB.Common.Core.Logging
{
    public class HealthMonitoringWrapper : ILogger
    {
        public void Log(Exception e)
        {
            new LogEvent(e.Message, e.Source, WebEventCodes.WebExtendedBase).Raise();
        }

        public void Log(string message, object source)
        {
            new LogEvent(message, source, WebEventCodes.WebExtendedBase).Raise();
        }

        private class LogEvent : WebAuditEvent
        {
            public LogEvent(string msg, object eventSource, int eventCode)
                : base(msg, eventSource, eventCode)
            {
                Debug.WriteLine("»»" + msg.Substring(0, msg.Length > 128 ? 128 : msg.Length) + "...|" + eventSource + "|" + eventCode);
            }

        }

    }
}