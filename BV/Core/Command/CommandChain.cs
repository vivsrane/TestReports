namespace VB.Common.Core.Command
{
    public abstract class CommandChain<TResult, TParameter> : ICommand
    {
        private readonly ICommandParameter<TParameter> _parameter;
        private readonly ICommandResult<TResult> _result;

        protected CommandChain(ICommandParameter<TParameter> parameter, ICommandResult<TResult> result)
        {
            _parameter = parameter;
            _result = result;
        }

        protected ICommandParameter<TParameter> Parameter
        {
            get { return _parameter; }
        }

        protected ICommandResult<TResult> Result
        {
            get { return _result; }
        }

        public abstract void Execute();
    }
}