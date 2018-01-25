namespace VB.Common.Core.Command
{
    /// <summary>
    /// Base cached-command class.  A cache and logger are provided to children.
    /// </summary>
    public abstract class CachedCommand : AbstractCommand
    {
        // Injected dependencies
        public ICache Cache { get; set; }
        public ILogger Log { get; set; }

        // Ctor
        protected CachedCommand()
        {
            // use injected dependencies
        }

        protected CachedCommand(ICache cache, ILogger logger)
        {
            // Use the supplied values unless they are null.
            if( cache != null )
            {
                Cache = cache;
            }

            if( logger != null )
            {
                Log = logger;
            }

            // Otherwise, the injected values will be used.
        }

        protected int DefaultCacheExpirationSeconds
        {
            get
            {
                return 10;
            }
        }


    }
}
