namespace VB.Common.Core.Command
{
    public class SimpleCommandResult<TResult> : ICommandResult<TResult>
    {
        private readonly TResult _result;

        public SimpleCommandResult(TResult result)
        {
            _result = result;
        }

        public TResult Result
        {
            get { return _result; }
        }
    }
}