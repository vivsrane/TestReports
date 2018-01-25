using System;

namespace VB.Common.Core
{
    /// <summary>
    /// A logger which does nothing.
    /// </summary>
    public class NullLogger : ILogger 
    {
        public void Log(Exception e)
        {
        }

        public void Log(string message, object source)
        {
        }
    }
}
