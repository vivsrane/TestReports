using System.Diagnostics.CodeAnalysis;

namespace VB.Common.Threading
{
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Is a minimal Queue interface")]
    public abstract class WaitQueue
    {
        public abstract void Insert(WaitNode node);

        public abstract WaitNode Extract();
    }
}
