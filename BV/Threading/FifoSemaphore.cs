using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Threading
{
    public class FifoSemaphore : QueuedSemaphore
    {
        public FifoSemaphore(long initialPermits)
            : base(new FifoWaitQueue(), initialPermits)
        {
        }

        [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Is a Queue")]
        protected class FifoWaitQueue : WaitQueue
        {
            WaitNode head_;
            WaitNode tail_;

            public override void Insert(WaitNode node)
            {
                if (tail_ == null)
                {
                    head_ = tail_ = node;
                }
                else
                {
                    tail_.Next = node;
                    tail_ = node;
                }
            }

            public override WaitNode Extract()
            {
                if (head_ == null)
                {
                    return null;
                }
                else
                {
                    WaitNode w = head_;
                    head_ = w.Next;
                    if (head_ == null) tail_ = null;
                    w.Next = null;
                    return w;
                }
            }
        }
    }
}
