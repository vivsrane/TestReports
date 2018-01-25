using System;

namespace VB.Common.Core.Command
{
    [Serializable]
    public class Command<TResult, TParameter> : ICommand, ICommandParameter<TParameter>, ICommandResult<TResult>
    {
        private readonly ICommand<TResult, TParameter> _command;
        private readonly TParameter _parameter;
        private TResult _result;

        public Command(ICommand<TResult, TParameter> command, TParameter parameter)
        {
            _command = command;
            _parameter = parameter;
        }

        public void Execute()
        {
            _result = _command.Execute(_parameter);
        }

        public TParameter Parameter { get { return _parameter; } }

        public TResult Result { get { return _result; } }
    }
}