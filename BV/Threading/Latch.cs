using System.Threading;

namespace VB.Common.Threading
{
    public class Latch
    {
        private bool latched; // false by default

        public void Acquire()
        {
            lock (this) { while (!latched) Monitor.Wait(this); }
        }

        public void Release()
        {
            lock (this) { latched = true; Monitor.PulseAll(this); }
        }
    }
}
