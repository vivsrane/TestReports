using System;

namespace VB.Common.Threading
{
    public abstract class QueuedSemaphore : Semaphore
    {
        private readonly WaitQueue _queue;

        protected QueuedSemaphore(WaitQueue queue, long initialPermits)
            : base(initialPermits)
        {
            _queue = queue;
        }

        protected WaitQueue Queue
        {
            get { return _queue; }
        }

        public override void Take()
        {
            if (PreCheck())
                return;
            WaitNode w = new WaitNode();
            w.DoWait(this);
        }

        public override bool Take(TimeSpan timeout)
        {
            if (PreCheck())
                return true;
            if (timeout.TotalMilliseconds <= 0)
                return false;
            WaitNode w = new WaitNode();
            return w.DoTimedWait(this, timeout);
        }

        internal bool PreCheck()
        {
            lock (this)
            {
                bool pass = (Permits > 0);
                if (pass) --Permits;
                return pass;
            }
        }

        internal bool ReCheck(WaitNode w)
        {
            lock (this)
            {
                bool pass = (Permits > 0);
                if (pass) --Permits;
                else Queue.Insert(w);
                return pass;
            }
        }

        internal WaitNode GetSignallee()
        {
            lock (this)
            {
                WaitNode w = Queue.Extract();
                if (w == null) ++Permits; // if none, inc permits for new arrivals
                return w;
            }
        }

        public override void Release()
        {
            while (true)
            {
                WaitNode w = GetSignallee();
                if (w == null) return;  // no one to signal
                if (w.Signal()) return; // notify if still waiting, else skip
            }
        }

        public override void Release(long numberOfPermits)
        {
            if (numberOfPermits < 0)
                throw new ArgumentException("Negative argument", "numberOfPermits");

            for (long i = 0; i < numberOfPermits; ++i)
                Release();
        }
    }
}
