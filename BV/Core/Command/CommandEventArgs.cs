using System;

namespace VB.Common.Core.Command
{
    public class CommandEventArgs : EventArgs
    {
        private readonly Type _commandType;

        public CommandEventArgs(Type commandType)
        {
            _commandType = commandType;
        }

        public Type CommandType
        {
            get { return _commandType; }
        }
    }
}
