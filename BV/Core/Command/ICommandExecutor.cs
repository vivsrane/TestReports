namespace VB.Common.Core.Command
{
    public interface ICommandExecutor : ICommandObservable
    {
        ICommandResult<TResult> Execute<TResult, TParameter>(
            ICommand<TResult, TParameter> command,
            TParameter parameter);
    }
}