using System;

namespace VB.Reports.App.ReportDefinitionLibrary.Xml
{
    internal class DateRange
    {
        TimeUnit initialUnit;
        int initialDistance;
        TimeUnit distanceUnit;
        int distance;
        int numberOfItems;
        TimeDirection direction;

        public TimeUnit InitialUnit
        {
            get { return initialUnit; }
            set { initialUnit = value; }
        }

        public int InitialDistance
        {
            get { return initialDistance; }
            set { initialDistance = value; }
        }

        public TimeUnit DistanceUnit
        {
            get { return distanceUnit; }
            set { distanceUnit = value; }
        }

        public int Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public int NumberOfItems
        {
            get { return numberOfItems; }
            set { numberOfItems = value; }
        }

        public TimeDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public DateTime[] Items()
        {
            DateTime[] items = new DateTime[NumberOfItems];

            items[0] = Truncate(InitialUnit, DateTime.Now);

            if (InitialDistance > 0)
                items[0] = Move(DistanceUnit, InitialDistance, Direction, items[0]);

            for (int i = 1; i < NumberOfItems; i++)
            {
                items[i] = Move(DistanceUnit, Distance, Direction, items[i - 1]);
            }

            return items;
        }

        public static DateTime Move(TimeUnit u, int x, TimeDirection r, DateTime item)
        {
            DateTime value;

            int i = (r.Equals(TimeDirection.Backward) ? -1 : 1) * x;

            switch (u)
            {
                case TimeUnit.YY:
                    value = item.AddYears(i);
                    break;
                case TimeUnit.MM:
                    value = item.AddMonths(i);
                    break;
                case TimeUnit.DD:
                    value = item.AddDays(i);
                    break;
                case TimeUnit.HH:
                    value = item.AddHours(i);
                    break;
                case TimeUnit.MI:
                    value = item.AddMinutes(i);
                    break;
                default:
                    throw new ArgumentException("Not a TimeUnit: " + u);
            }

            return value;
        }

        public static DateTime Truncate(TimeUnit unit, DateTime item)
        {
            DateTime value;

            switch (unit)
            {
                case TimeUnit.YY:
                    value = new DateTime(item.Year, 1, 1, 0, 0, 0, 0);
                    break;
                case TimeUnit.MM:
                    value = new DateTime(item.Year, item.Month, 1, 0, 0, 0, 0);
                    break;
                case TimeUnit.DD:
                    value = new DateTime(item.Year, item.Month, item.Day, 0, 0, 0, 0);
                    break;
                case TimeUnit.HH:
                    value = new DateTime(item.Year, item.Month, item.Day, item.Hour, 0, 0, 0);
                    break;
                case TimeUnit.MI:
                    value = new DateTime(item.Year, item.Month, item.Day, item.Hour, item.Minute, 0, 0);
                    break;
                default:
                    throw new ArgumentException("Not a TimeUnit: " + unit);
            }

            return value;
        }
    }

    internal enum TimeUnit
    {
        YY = 5, // biggest time unit
        MM = 4,
        DD = 3,
        HH = 2,
        MI = 1  // smallest time unit
    }

    internal enum TimeDirection
    {
        Backward = 0,
        Forward  = 1
    }
}
