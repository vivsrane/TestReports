using System;
using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Threading
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = ".NET does not define a Queue interface")]
    public interface IQueue<T>
    {
        T Peek();

        T Poll();

        T Poll(TimeSpan timeout);

        bool Offer(T value);

        bool Offer(T value, TimeSpan timeout);

        bool IsEmpty();

        int Size();
    }
}
