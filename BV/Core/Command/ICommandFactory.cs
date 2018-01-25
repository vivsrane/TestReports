namespace VB.Common.Core.Command
{
    public interface ICommandFactory
    {
        ICommandTransaction CreateTransactionCommand();
    }
}