namespace VB.Common.Core
{
    public class AsyncState<TParameters,TResult>
    {
        private readonly TParameters parameters;
        private readonly AsyncResult<TResult> result;
        
        public AsyncState(TParameters parameters, AsyncResult<TResult> result)
        {
            this.parameters = parameters;
            this.result = result;
        }

        public TParameters Parameters
        {
            get { return parameters; }
        }

        public AsyncResult<TResult> Result
        {
            get { return result; }
        }
    }
}