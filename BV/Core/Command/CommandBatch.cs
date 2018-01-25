using System;

namespace VB.Common.Core.Command
{
    [Serializable]
    public class CommandBatch : ICommand
    {
        private readonly ICommand[] _cs;

        public CommandBatch(params ICommand[] cs)
        {
            _cs = cs;
        }

        public void Execute()
        {
            foreach (ICommand c in _cs)
            {
                c.Execute();
            }
        }
    }
}