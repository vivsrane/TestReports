using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;

namespace VB.Common.Threading
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Implements IQueue")]
    public class LinkedQueue<T> : IQueue<T> where T : class
    {
        private readonly object offerLock = new object();

        private LinkedNode<T> head;

        private LinkedNode<T> last;

        private int offerCount = 0;

        private int pollCount = 0;

        private int waitingForPoll = 0;

        public LinkedQueue()
        {
            head = new LinkedNode<T>(default(T));
            last = head;
        }

        public int Size()
        {
            lock (this)
            {
                return offerCount - pollCount;
            }
        }

        protected void Insert(T value)
        {
            lock (offerLock)
            {
                LinkedNode<T> p = new LinkedNode<T>(value);
                lock (last)
                {
                    last.Next = p;
                    last = p;
                    Interlocked.Increment(ref offerCount);
                }
                if (waitingForPoll > 0)
                {
                    Monitor.Pulse(offerLock);
                }
            }
        }

        protected T Extract()
        {
            lock (this)
            {
                lock (head)
                {
                    T x = null;
                    LinkedNode<T> first = head.Next;
                    if (first != null)
                    {
                        x = first.Value;
                        first.Value = null;
                        head = first;
                        Interlocked.Increment(ref pollCount);
                    }
                    return x;
                }
            }
        }

        public bool Offer(T value)
        {
            if (value == null) throw new ArgumentNullException("value");
            Insert(value);
            return true;
        }

        public bool Offer(T value, TimeSpan timeout)
        {
            if (value == null) throw new ArgumentNullException("value");
            Insert(value);
            return true;
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
                lock (offerLock)
                {
                    try
                    {
                        ++waitingForPoll;
                        while (true)
                        {
                            x = Extract();
                            if (x != null)
                            {
                                --waitingForPoll;
                                return x;
                            }
                            else
                            {
                                Monitor.Wait(offerLock);
                            }
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        --waitingForPoll;
                        Monitor.Pulse(offerLock);
                        throw;
                    }
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
                    return null;
            }
        }

        public bool IsEmpty()
        {
            lock (head)
            {
                return head.Next == null;
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
                lock (offerLock)
                {
                    try
                    {
                        TimeSpan waitTime = timeout;

                        DateTime start = DateTime.Now;

                        ++waitingForPoll;

                        while (true)
                        {
                            x = Extract();

                            if (x != null || waitTime.Ticks <= 0)
                            {
                                --waitingForPoll;

                                return x;
                            }
                            else
                            {
                                Monitor.Wait(offerLock, waitTime);

                                waitTime = timeout.Subtract(DateTime.Now.Subtract(start));
                            }
                        }
                    }
                    catch (ThreadInterruptedException)
                    {
                        --waitingForPoll;
                        Monitor.Pulse(offerLock);
                        throw;
                    }
                }
            }
        }
    }
}
