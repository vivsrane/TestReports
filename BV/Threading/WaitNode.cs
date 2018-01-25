using System;
using System.Threading;

namespace VB.Common.Threading
{
    public class WaitNode
    {
        private bool waiting = true;

        private WaitNode next;

        public WaitNode Next
        {
            get { return next; }
            set { next = value; }
        }

        public bool Signal()
        {
            lock (this)
            {
                bool signalled = waiting;
                if (signalled)
                {
                    waiting = false;
                    Monitor.Pulse(this);
                }
                return signalled;
            }
        }

        public bool DoTimedWait(QueuedSemaphore semaphore, TimeSpan timeout)
        {
            lock (this)
            {
                if (semaphore.ReCheck(this) || !waiting)
                {
                    return true;
                }
                else if (timeout.Ticks <= 0)
                {
                    waiting = false;
                    return false;
                }
                else
                {
                    TimeSpan waitTime = timeout;

                    DateTime start = DateTime.Now;

                    try
                    {
                        while (true)
                        {
                            Monitor.Wait(this, waitTime);
                            if (!waiting)
                            {
                                return true; // signalled
                            }
                            else
                            {
                                waitTime = timeout.Subtract(DateTime.Now.Subtract(start));
                                if (waitTime.Ticks <= 0)
                                {
                                    waiting = false; // timed out
                                    return false;
                                }
                            }
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        if (waiting)
                        {
                            // no notification
                            waiting = false; // invalidate for the signaller
                            throw;
                        }
                        else
                        {
                            // thread was interrupted after it was notified
                            Thread.CurrentThread.Interrupt();
                            return true;
                        }
                    }
                }
            }
        }

        public void DoWait(QueuedSemaphore semaphore)
        {
            lock (this)
            {
                if (!semaphore.ReCheck(this))
                {
                    try
                    {
                        while (waiting) Monitor.Wait(this);
                    }
                    catch (ThreadInterruptedException)
                    {
                        if (waiting)
                        {
                            // no notification
                            waiting = false; // invalidate for the signaller
                            throw;
                        }
                        else
                        {
                            // thread was interrupted after it was notified
                            Thread.CurrentThread.Interrupt();
                            return;
                        }
                    }
                }
            }
        }
    }
}
