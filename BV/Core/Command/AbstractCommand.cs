namespace VB.Common.Core.Command
{
    /// <summary>
    /// Base command class.
    /// </summary>
    public abstract class AbstractCommand
    {
        // Ctor
        protected AbstractCommand()
        {
            // Inject unset dependencies.
            IOC.Registry.BuildUp(this);
        }

        /// <summary>
        /// Children may implement pre-run behavior by implementing this method.
        /// </summary>
        protected virtual void DoPreRun()
        {}

        /// <summary>
        /// Clients will invoke commands via this method.
        /// </summary>
        /// <param name="command"></param>
        public static void DoRun(AbstractCommand command)
        {
            command.DoPreRun();
            command.Run();
            command.DoPostRun();
        }

        /// <summary>
        /// Children may implement post-run behavior by implementing this method.
        /// </summary>
        protected virtual void DoPostRun()
        {}

        /// <summary>
        /// Children must implement run.  This is the definition of what the command does.
        /// </summary>
        protected abstract void Run();

    }
}
