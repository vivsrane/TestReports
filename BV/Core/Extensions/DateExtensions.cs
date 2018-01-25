using System;

namespace VB.Common.Core.Extensions
{
    public static class DateExtensions
    {
        public static DateTime FirstSecondOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }

        public static DateTime LastSecondOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1).AddMonths(1).AddSeconds(-1);
        }

        public static DateTime StartOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day);
        }

        public static DateTime EndOfDay(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day).AddDays(1).AddSeconds(-1);
        }

        /// <summary>
        /// Converts a DateTime to be used for data persistance (Enforce UTC)
        /// </summary>
        /// <param name="time">The DateTime to be persisted</param>
        /// <returns>A persistence layer ready DateTime</returns>
        public static DateTime InputTime(this DateTime time)
        {
            if (time.Kind == DateTimeKind.Unspecified)
            {
                throw new ArgumentException("Time values cannot be input to the data store with unspecified kind.");
            }
            return time.ToUniversalTime();
        }

        /// <summary>
        /// Converts a data persistance layer DateTime into a logic ready DateTime (Return to UTC)
        /// </summary>
        /// <param name="time">The DateTime provided by a data persistance layer</param>
        /// <param name="tzInfo">The timezone of the provided dateTime to be used if dateTime kind is unspecified</param>
        /// <returns>The DateTime to be used in code logic</returns>
        public static DateTime OutputTime(this DateTime time, TimeZoneInfo tzInfo)
        {
            if (time.Kind == DateTimeKind.Unspecified)
            {
                time = TimeZoneInfo.ConvertTimeToUtc(time, tzInfo);
            }
            return time.ToUniversalTime();
        }
    }
}
