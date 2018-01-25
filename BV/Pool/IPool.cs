using System;

namespace VB.Common.Pool
{
    /// <summary>
    /// A simple pooling interface. Is used in conjunction with a <code>PooledItemFactory</code>
    /// that defines the operations performed as part of an object's life-cycle in the pool.
    /// </summary>
    /// <example>
    /// E item = null;
    /// try {
    ///     item = pool.BorrowObject(new TimeSpan(TimeSpan.TicksPerSecond));
    ///     if (item != null) {
    ///         /* use object */
    ///     }
    /// }
    /// catch (PoolException e) {
    ///     /* handle exception */
    /// }
    /// finally {
    ///     /* return object to the pool */
    ///     if (item != null) {
    ///         pool.ReturnItem(item);
    ///     }
    /// }
    /// </example>
    /// <typeparam name="TItem">The type of pooled object</typeparam>
    public interface IPool<TItem> where TItem : class
    {
        /// <summary>
        /// Name of the pool.
        /// </summary>
        string InstanceName
        {
            get;
        }

        /// <summary>
        /// Factory that is used to create new object instances.
        /// </summary>
        IPooledItemFactory<TItem> Factory
        {
            get;
        }

        /// <summary>
        /// Obtain an object from the pool.
        /// </summary>
        /// <returns>object from the pool, or null if the specified waiting
        /// time elapses before an element is present.</returns>
        /// <exception cref="PoolClosedException">pool is closed</exception>
        /// <exception cref="PoolLifeCycleException">could not activate or validate
        /// the next available item from the pool</exception>
        /// <exception cref="PoolImplementationException">if the borrow-item life-cycle cannot be
        /// completed on a new object from the factory</exception>
        TItem BorrowItem();

        /// <summary>
        /// Obtain an instance from the pool, waiting if necessary up
        /// to the specified wait time for one to become available.
        /// </summary>
        /// <param name="timeout">how long to wait before giving up</param>
        /// <returns>object from the pool, or null if the specified waiting
        /// time elapses before an element is present.</returns>
        /// <exception cref="ArgumentException">timeout < -1</exception>
        /// <exception cref="PoolClosedException">pool is closed</exception>
        /// <exception cref="PoolLifeCycleException">could not activate or validate
        /// the next available item from the pool</exception>
        /// <exception cref="PoolImplementationException">if the borrow-item life-cycle cannot be
        /// completed on a new object from the factory</exception>
        TItem BorrowItem(TimeSpan timeout);

        /// <summary>
        /// Return an instance to the pool. The object must have been obtained
        /// from the pool to which it is returned.
        /// </summary>
        /// <param name="item">a borrowed instance to be returned</param>
        /// <exception cref="ArgumentNullException">obj is null</exception>
        /// <exception cref="PoolClosedException">pool is closed</exception>
        /// <exception cref="ArgumentException">obj not borrowed from this pool</exception>
        void ReturnItem(TItem item);

        /// <summary>
        /// Clears any objects sitting idle in the pool, releasing any associated
        /// resources.
        /// </summary>
        /// <exception cref="PoolClosedException">pool is closed</exception>
        void Clear();

        /// <summary>
        /// Close this pool, and free any resources associated with it.
        /// </summary>
        void Close();
    }
}
