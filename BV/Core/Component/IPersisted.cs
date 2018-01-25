namespace VB.Common.Core.Component
{
    public interface IPersisted
    {
        /// <summary>
        /// Is this entity new?
        /// </summary>
        bool IsNew { get; }

        /// <summary>
        /// Has the entity been marked for deletion?
        /// </summary>
        bool IsDeleted { get; }

        /// <summary>
        /// Is the entity dirty?
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Mark this entity as being up for deletion.
        /// </summary>
        void MarkDeleted();

        /// <summary>
        /// Mark this entity as old
        /// </summary>
        void MarkOld();

        /// <summary>
        /// Mark this entity as new
        /// </summary>
        void MarkNew();
    }
}
