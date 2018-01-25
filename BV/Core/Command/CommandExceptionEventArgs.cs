using System;

namespace VB.Common.Core.Command
{
    public class CommandExceptionEventArgs : CommandEventArgs
    {
        private readonly Exception _exception;

        public CommandExceptionEventArgs(Type commandType, Exception exception) : base(commandType)
        {
            _exception = exception;
        }

        public Exception Exception
        {
            get { return _exception; }
        }

    }
}
