using System;
using VB.Common.Core.IOC;

namespace Csla
{
    /// <summary>
    /// A read only list base which performs (setter) dependency injection.
    /// </summary>
    /// <typeparam name="T">Type of the business object being defined.</typeparam>
    /// <typeparam name="C">Type of the child objects contained in the list.</typeparam>
    [Serializable]
    public abstract class InjectableReadOnlyListBase<T,C> : ReadOnlyListBase<T,C>
        where T : ReadOnlyListBase<T,C>
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
