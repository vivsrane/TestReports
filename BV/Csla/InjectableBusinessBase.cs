using System;
using VB.Common.Core.IOC;

namespace Csla
{
    /// <summary>
    /// A business base which performs (setter) dependency injection.
    /// </summary>
    /// <typeparam name="T">Type of the business object being defined.</typeparam>
    [Serializable]
    public abstract class InjectableBusinessBase<T> : BusinessBase<T> where T : BusinessBase<T>
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

        /// <summary>
        /// This method is called on a newly deserialized object after deserialization is complete.
        /// Setter injection is performed before the object is returned to the caller.
        /// </summary>
        /// <param name="context">Serialization context object.</param>
        protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            Registry.BuildUp(this);
            base.OnDeserialized(context);
        }
    }
}
