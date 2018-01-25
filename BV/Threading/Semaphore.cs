using System;
using System.Globalization;
using System.Threading;

namespace VB.Common.Threading
{
    public class Semaphore
    {
        private long _permits;

        /// <summary>
        /// Create a Semaphore with the given initial number of permits.
        /// Using a seed of one makes the semaphore act as a mutual exclusion lock.
        /// Negative seeds are also allowed, in which case no acquires will proceed
        /// until the number of releases has pushed the number of permits past 0.
        /// </summary>
        /// <param name="initialPermits"></param>
        public Semaphore(long initialPermits)
        {
            _permits = initialPermits;
        }

        /// <summary>
        /// Current number of available permits.
        /// </summary>
        protected long Permits
        {
            get { return _permits; }
            set { _permits = value; }
        }

        /// <summary>
        /// Wait until a permit is available, and take one.
        /// </summary>
        public virtual void Take()
        {
            lock (this)
            {
                try
                {
                    while (Permits <= 0)
                        Monitor.Wait(this);
                    --Permits;
                }
                catch (ThreadInterruptedException)
                {
                    Monitor.Pulse(this);
                    throw;
                }
            }
        }

        public virtual bool Take(TimeSpan timeout)
        {
            if (timeout.Ticks < Timeout.Infinite)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentUICulture, "Timeout must be greater or equal to Timeout.Infinite ({0}) ticks", Timeout.Infinite),
                    "timeout");
            }

            if (timeout.Ticks == Timeout.Infinite)
            {
                Take();

                return true;
            }

            lock (this)
            {
                if (Permits > 0)
                {
                    --Permits;
                    return true;
                }
                else if (timeout.Ticks == 0)
                {
                    return false;
                }
                else
                {
                    try
                    {
                        TimeSpan waitTime = timeout;

                        DateTime startTime = DateTime.Now;

                        while (true)
                        {
                            Monitor.Wait(this, waitTime);

                            if (Permits > 0)
                            {
                                --Permits;
                                return true;
                            }
                            else
                            {
                                waitTime = timeout.Subtract(DateTime.Now.Subtract(startTime));

                                if (waitTime.Ticks <= 0)
                                    return false;
                            }
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        Monitor.Pulse(this);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Release a permit.
        /// </summary>
        public virtual void Release()
        {
            lock (this)
            {
                ++Permits;

                Monitor.Pulse(this);
            }
        }

        /// <summary>
        /// Release N permits.
        /// </summary>
        /// <param name="numberOfPermits"></param>
        public virtual void Release(long numberOfPermits)
        {
            lock (this)
            {
                if (numberOfPermits < 0) throw new ArgumentException("Negative argument", "numberOfPermits");

                Permits += numberOfPermits;

                for (long i = 0; i < numberOfPermits; ++i)
                {
                    Monitor.Pulse(this);
                }
            }
        }

        /// <summary>
        /// Return the current number of available permits.
        /// Returns an accurate, but possibly unstable value,
        /// that may change immediately after returning.
        /// </summary>
        /// <returns></returns>
        public virtual long AvailablePermits()
        {
            lock (this)
            {
                return Permits;
            }
        }
    }
}
