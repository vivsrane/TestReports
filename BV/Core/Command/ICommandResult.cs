namespace VB.Common.Core.Command
{
    public interface ICommandResult<TResult>
    {
        TResult Result { get; }
    }
}