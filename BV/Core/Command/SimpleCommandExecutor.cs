using System.Collections.Generic;

namespace VB.Common.Core.Command
{
    public class SimpleCommandExecutor : ICommandExecutor
    {

        private readonly List<ICommandObserver> _observers = new List<ICommandObserver>();  


        public ICommandResult<TResult> Execute<TResult, TParameter>(ICommand<TResult, TParameter> command, TParameter parameter)
        {
            try
            {

                NotifyInvoke(new CommandEventArgs(command.GetType()));

                TResult result = command.Execute(parameter);

                NotifyInvokeComplete(new CommandEventArgs(command.GetType()));

                return new SimpleCommandResult<TResult>(result);
            }
            catch (System.Exception e)
            {

                NotifyInvokeException(new CommandExceptionEventArgs(command.GetType(),e));

                throw;
            }
        }

        #region ICommandExecutor Members


        public void Add(ICommandObserver observer)
        {
            _observers.Add(observer);

        }

        public void Remove(ICommandObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotifyInvoke(CommandEventArgs e)
        {

            foreach (ICommandObserver observer in _observers)
            {
                try
                {
                    observer.OnInvoke(e);
                }
                catch
                {
                }
            }
    
        }

        public void NotifyInvokeComplete(CommandEventArgs e)
        {
            foreach (ICommandObserver observer in _observers)
            {
                try
                {
                    observer.OnInvokeComplete(e);
                }
                catch
                {
                }
            }

        }

        public void NotifyInvokeException(CommandExceptionEventArgs e)
        {
            foreach (ICommandObserver observer in _observers)
            {
                try
                {
                    observer.OnInvokeException(e);
                }
                catch
                {
                }
            }

        }


        #endregion
       

    }
}