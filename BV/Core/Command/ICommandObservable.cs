
namespace VB.Common.Core.Command
{
    public interface ICommandObservable
    {
        void Add(ICommandObserver observer);

        void Remove(ICommandObserver observer);

        void NotifyInvoke(CommandEventArgs e);

        void NotifyInvokeComplete(CommandEventArgs e);

        void NotifyInvokeException(CommandExceptionEventArgs e);
    }
}
