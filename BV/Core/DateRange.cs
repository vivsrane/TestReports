using System;
using System.Collections.Generic;
using VB.Common.Core.Extensions;

namespace VB.Common.Core
{
    /// <summary>
    /// Represents a list of Months, useful for the date ranges we use on the dashboards.
    /// </summary>
    public class DateRange : IEquatable<DateRange>, IEqualityComparer<DateRange>
    {
        private DateTime _startDate;
        private DateTime _endDate;

        public DateTime StartDate { get { return _startDate; } }
        public DateTime EndDate { get { return _endDate; } }

        #region Static Constructors

        /// <summary>
        /// A DateRange representing the time from the first second to the last second of the month.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateRange MonthOfDate(DateTime date)
        {
            return new DateRange(date.FirstSecondOfMonth(), date.LastSecondOfMonth());
        }

        /// <summary>
        /// Convenience method for the year prior to a given date.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateRange YearBeforeDate(DateTime dateTime)
        {
            DateTime startDate = DateTime.Now.AddYears(-1);
            return new DateRange(startDate, DateTime.Now);
        }

        /// <summary>
        /// Get a range by number of months given a starting date.
        /// </summary>
        /// <param name="months"></param>
        /// <param name="fromDate"></param>
        /// <returns></returns>
        public static DateRange MonthsFromDate(int months, DateTime fromDate)
        {
            if (months >= 0)
                return new DateRange(fromDate, fromDate.AddMonths(months));
            else
                return new DateRange(fromDate.AddMonths(months), fromDate);
        }

        #endregion Static Constructors

        #region Instance Constructors

        // An empty Default constructor
        public DateRange() { }

        /// <summary>
        /// Build a DateRange from the first second of the start year/month to the last second of the end year/month. Likely our most popular constructor.
        /// </summary>
        /// <param name="startYear"></param>
        /// <param name="startMonth"></param>
        /// <param name="endYear"></param>
        /// <param name="endMonth"></param>
        public DateRange(int startYear, int startMonth, int endYear, int endMonth) :
            this(
                new DateTime(startYear, startMonth, 1),
                new DateTime(endYear, endMonth, DateTime.DaysInMonth(endYear, endMonth)).LastSecondOfMonth()
            ) { }

        [Newtonsoft.Json.JsonConstructor]
        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("StartDate must be before EndDate");

            _startDate = startDate.StartOfDay();
            _endDate = endDate.EndOfDay();
        }

        #endregion Instance Constructors

        #region IEquatable Implementation

        // Required for Linq.Contains
        public bool Equals(DateRange obj)
        {
            if (Object.ReferenceEquals(this, obj)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(this, null) || Object.ReferenceEquals(obj, null))
                return false;

            return _startDate.Equals(obj.StartDate) && _endDate.Equals(obj.EndDate);
        }

        // Required for Linq.Except
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + _startDate.GetHashCode();
                hash = hash * 23 + _endDate.GetHashCode();
                return hash;
            }
        }

        #endregion IEquatable Implementation

        #region IEqualityComparer Implementation
        public bool Equals(DateRange x, DateRange y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(DateRange obj)
        {
            return obj.GetHashCode();
        }
        #endregion  IEqualityComparer Implementation

        #region Overrides and Operators

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        // Provided for convenience
        public static bool operator ==(DateRange dr1, DateRange dr2)
        {
            return dr1.Equals(dr2);
        }
        public static bool operator !=(DateRange dr1, DateRange dr2)
        {
            return !dr1.Equals(dr2);
        }

        // Universally sortable human readable expression of the range.
        public override string ToString()
        {
            return String.Format("{0} - {1}", StartDate.ToString("u"), EndDate.ToString("u"));
        }

        #endregion Overrides and Operators

    }

    /// <summary>
    /// Extension methods to make DateRanges Linq queryable
    /// </summary>
    public static class DateRangeExtensions
    {
        public static IEnumerable<DateRange> AsDays(this DateRange dateRange)
        {
            DateTime day = dateRange.StartDate;
            while (day <= dateRange.EndDate)
            {
                yield return new DateRange(day, day);
                day = day.AddDays(1);
            }
        }

        public static IEnumerable<DateRange> AsWeeks(this DateRange dateRange)
        {
            DateTime day = dateRange.StartDate;
            while (day <= dateRange.EndDate)
            {
                yield return new DateRange(day, day.AddDays(7).AddSeconds(-1));
                day = day.AddDays(7);
            }
        }

        public static IEnumerable<DateRange> AsMonths(this DateRange dateRange)
        {
            DateTime day = new DateTime(dateRange.StartDate.Year, dateRange.StartDate.Month, dateRange.StartDate.Day);
            while (day <= dateRange.EndDate)
            {
                yield return DateRange.MonthOfDate(day);
                day = day.AddMonths(1);
            }
        }

	    public static bool Contains(this DateRange dateRange, DateTime compareDateTime)
	    {
		    return dateRange.StartDate <= compareDateTime && dateRange.EndDate >= compareDateTime;
	    }
    }
}
