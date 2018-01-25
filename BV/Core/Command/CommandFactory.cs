using VB.Common.Core.Data;

namespace VB.Common.Core.Command
{
    public abstract class CommandFactory : ICommandFactory
    {
        protected class CommandTransaction : ICommandTransaction
        {
            private readonly ITransactionFactory _factory;

            public CommandTransaction(ITransactionFactory factory)
            {
                _factory = factory;
            }

            public void Execute(ICommand command)
            {
                using (ITransaction transaction = _factory.BeginTransaction())
                {
                    command.Execute();

                    transaction.Commit();
                }

                _factory.Dispose();
            }
        }

        public abstract ICommandTransaction CreateTransactionCommand();
    }
}