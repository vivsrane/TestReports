namespace VB.Common.Core.Diagnostics.Counters
{

    abstract public class BasePerformanceCounter : IPerformanceCounter
    {

        protected System.Diagnostics.PerformanceCounter Counter;
        protected System.Diagnostics.PerformanceCounter BaseCounter;
        protected PerformanceCounterType Type;
        protected PerformanceCounterCategory Category;

        #region " properties "

        public string Name
        {
            get { return Counter.InstanceName; }
        }

        public string Description
        {
            get { return Counter.CounterHelp; }
        }

        public PerformanceCounterCategory CounterCategory
        {
            get { return Category; }
        }

        public PerformanceCounterType CounterType
        {
            get { return Type; }
        }

        #endregion


        #region " constructors "

        protected BasePerformanceCounter() { }

        protected BasePerformanceCounter(string name, PerformanceCounterType type, PerformanceCounterCategory category)
        {
            Type = type;
            Category = category;
            Counter = new System.Diagnostics.PerformanceCounter(category.ToString(), type.ToString(), name, false);
            Counter.RawValue = 0;

            if (IsBaseCounterRequired(type))
            {
                BaseCounter = new System.Diagnostics.PerformanceCounter(category.ToString(), string.Format("{0}Base", type), string.Format("{0}Base", name), false);
                BaseCounter.RawValue = 0;
            }
        }

        #endregion


        #region " interface implementation "

        public abstract long Increment();

        public abstract long IncrementBy(long value);

        public void Dispose()
        {
            Counter.RemoveInstance();
        }

        #endregion


        #region " helpers "

        protected static bool IsBaseCounterRequired(PerformanceCounterType counterType)
        {
            bool result = false;

            if (counterType == PerformanceCounterType.AverageOperationTime || counterType == PerformanceCounterType.OperationSuccessRatio)
            {
                result = true;
            }

            return result;
        }

        #endregion

    }

}