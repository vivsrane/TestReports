namespace VB.Common.Core.Command
{
    public interface ICommand
    {
        void Execute();
    }

    public interface ICommand<TResult,TParameter>
    {
        TResult Execute(TParameter parameters);
    }

    public interface ICommand<TParameter>
    {
        void Execute(TParameter parameter);
    }

    public interface ICommandTransaction
    {
        void Execute(ICommand command);
    }
}