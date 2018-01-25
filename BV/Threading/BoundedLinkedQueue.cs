using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace VB.Common.Threading
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Implements IQueue")]
    public class BoundedLinkedQueue<T> : IQueue<T> where T : class
    {
        private readonly object offerGuard = new object();

        private readonly object pollGuard = new object();

        private LinkedNode<T> head;

        private LinkedNode<T> tail;

        private int capacity;

        private int offerSideOfferPermits = 0;

        private int pollSideOfferPermits = 0;

        public BoundedLinkedQueue(int initialCapacity)
        {
            if (initialCapacity <= 0) throw new ArgumentException("Capactity must be greater than zero", "initialCapacity");
            capacity = initialCapacity;
            offerSideOfferPermits = initialCapacity;
            head = new LinkedNode<T>(default(T));
            tail = head;
        }

        protected int ReconcilePutPermits()
        {
            offerSideOfferPermits += pollSideOfferPermits;
            pollSideOfferPermits = 0;
            return offerSideOfferPermits;
        }

        public int Capacity()
        {
            lock (this)
            {
                return capacity;
            }
        }

        public int Size()
        {
            lock (this)
            {
                return capacity - (pollSideOfferPermits + offerSideOfferPermits);
            }
        }

        public bool IsEmpty()
        {
            lock (head)
            {
                return head.Next == null;
            }
        }

        public void SetCapacity(int newCapacity)
        {
            if (newCapacity <= 0)
                throw new ArgumentException("Capacity must be greater than zero");

            lock (offerGuard)
            {
                lock (this)
                {
                    // update capacity
                    pollSideOfferPermits += (newCapacity - capacity);
                    capacity = newCapacity;
                    // force immediate reconcilation.
                    ReconcilePutPermits();
                    Monitor.PulseAll(this);
                }
            }
        }

        protected T Extract()
        {
            lock (this)
            {
                lock (head)
                {
                    T x = default(T);
                    LinkedNode<T> first = head.Next;
                    if (first != null)
                    {
                        x = first.Value;
                        first.Value = default(T);
                        head = first;
                        ++pollSideOfferPermits;
                        Monitor.Pulse(this);
                    }
                    return x;
                }
            }
        }

        public T Peek()
        {
            lock (head)
            {
                LinkedNode<T> first = head.Next;
                if (first != null)
                    return first.Value;
                else
                    return default(T);
            }
        }

        public T Poll()
        {
            T x = Extract();

            if (x != null)
            {
                return x;
            }
            else
            {
                lock (pollGuard)
                {
                    try
                    {
                        while (true)
                        {
                            x = Extract();

                            if (x != null)
                            {
                                return x;
                            }
                            else
                            {
                                Monitor.Wait(pollGuard);
                            }
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        Monitor.Pulse(pollGuard);
                        throw;
                    }
                }
            }
        }

        public T Poll(TimeSpan timeout)
        {
            if (timeout.Ticks < Timeout.Infinite)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentUICulture, "Timeout must be greater or equal to Timeout.Infinite ({0}) ticks", Timeout.Infinite),
                    "timeout");
            }

            if (timeout.Ticks == Timeout.Infinite)
            {
                return Poll();
            }

            T x = Extract();

            if (x != null)
            {
                return x;
            }
            else
            {
                lock (pollGuard)
                {
                    try
                    {
                        TimeSpan waitTime = timeout;

                        DateTime start = DateTime.Now;

                        while (true)
                        {
                            x = Extract();

                            if (x != null || waitTime.Ticks <= 0)
                            {
                                return x;
                            }
                            else
                            {
                                Monitor.Wait(pollGuard, waitTime);

                                waitTime = timeout.Subtract(DateTime.Now.Subtract(start));
                            }
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        Monitor.Pulse(pollGuard);
                        throw;
                    }
                }
            }
        }

        protected void AllowTake()
        {
            lock (pollGuard)
            {
                Monitor.Pulse(pollGuard);
            }
        }

        protected void Insert(T value)
        {
            --offerSideOfferPermits;
            LinkedNode<T> p = new LinkedNode<T>(value);
            lock (tail)
            {
                tail.Next = p;
                tail = p;
            }
        }

        public bool Offer(T value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            lock (offerGuard)
            {
                if (offerSideOfferPermits <= 0)
                {
                    lock (this)
                    {
                        if (ReconcilePutPermits() <= 0)
                        {
                            try
                            {
                                while (true)
                                {
                                    Monitor.Wait(this);

                                    if (ReconcilePutPermits() > 0)
                                    {
                                        break;
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
                Insert(value);
            }
            AllowTake();
            return true;
        }

        public bool Offer(T value, TimeSpan timeout)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (timeout.Ticks < Timeout.Infinite)
            {
                throw new ArgumentException(
                    string.Format(CultureInfo.CurrentUICulture, "Timeout must be greater or equal to Timeout.Infinite ({0}) ticks", Timeout.Infinite),
                    "timeout");
            }

            if (timeout.Ticks == Timeout.Infinite)
            {
                return Offer(value);
            }

            lock (offerGuard)
            {
                if (offerSideOfferPermits <= 0)
                {
                    lock (this)
                    {
                        if (ReconcilePutPermits() <= 0)
                        {
                            if (timeout.Ticks <= 0)
                            {
                                return false;
                            }
                            else
                            {
                                try
                                {
                                    TimeSpan waitTime = timeout;

                                    DateTime start = DateTime.Now;

                                    while (true)
                                    {
                                        Monitor.Wait(this, waitTime);

                                        if (ReconcilePutPermits() > 0)
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            waitTime = timeout.Subtract(DateTime.Now.Subtract(start));

                                            if (waitTime.Ticks <= 0)
                                            {
                                                return false;
                                            }
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
                }
                Insert(value);
            }
            AllowTake();
            return true;
        }
    }
}