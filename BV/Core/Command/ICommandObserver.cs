
namespace VB.Common.Core.Command
{
    public interface ICommandObserver
    {
        void OnInvoke(CommandEventArgs e);

        void OnInvokeComplete(CommandEventArgs e);

        void OnInvokeException(CommandExceptionEventArgs e);
    }
}
