using System;
using VB.Common.Core.IOC;

namespace Csla
{
    /// <summary>
    /// A command base which performs (setter) dependency injection.
    /// </summary>
    [Serializable]
    public abstract class InjectableCommandBase : CommandBase
    {
        /// <summary>
        /// Called by the server-side DataPortal prior to calling the requested DataPortal_XYZ method.
        /// Setter injection is performed before the object is returned to the caller.
        /// </summary>
        /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
        protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
        {
            Registry.BuildUp(this);
            base.DataPortal_OnDataPortalInvoke(e);
        }
    }
}
