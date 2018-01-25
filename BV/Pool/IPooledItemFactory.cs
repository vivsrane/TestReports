namespace VB.Common.Pool
{
    /// <summary>
    /// An interface defining life-cycle methods for instances to be used in a pool.
    /// <list type="table">
    /// <listheader>
    /// <term>Pooled Item Life-Cycle</term>
    /// </listheader>
    /// <item><term>MakeItem</term>
    /// <description>Called to create new object instances for the pool</description></item>
    /// <item><term>ActivateItem</term>
    /// <description>Invoked on every instance before it is returned from its pool</description></item>
    /// <item><term>PassivateItem</term>
    /// <description>Invoked on every instance when it is returned to the pool</description></item>
    /// <item><term>DestroyItem</term>
    /// <description>Invoked on every instance when it is being "dropped" from the pool</description></item>
    /// <item><term>ValidateItem</term>
    /// <description>Invoked in an implementation-specific fashion to determine if an instance is still valid
    /// to be returned by the pool. It will only be invoked on an "activated" instance.</description></item>
    /// </list>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IPooledItemFactory<TItem>
    {
        /// <summary>
        /// Creates a object that can be returned by a pool.
        /// </summary>
        /// <returns>a new poolable object</returns>
        /// <exception cref="PoolException">If a new pooled item cannot be created</exception>
        TItem MakeItem();

        /// <summary>
        /// Destroys an object no longer needed by a pool.
        /// </summary>
        /// <param name="item">object to be destroyed</param>
        /// <exception cref="PoolLifecycleException">if the item cannot be destroyed</exception>
        void DestroyItem(TItem item);

        /// <summary>
        /// Ensures that an object is safe to be returned by a pool.
        /// </summary>
        /// <param name="item">object to be validated</param>
        /// <returns>true if the object is safe to be returned by a pool, false otherwise</returns>
        /// <exception cref="PoolLifecycleException">if the item cannot be validated</exception>
        bool ValidateItem(TItem item);

        /// <summary>
        /// Reinitialize an object to be returned by a pool.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="PoolLifecycleException">if the item cannot be activated</exception>
        void ActivateItem(TItem item);

        /// <summary>
        /// Uninitialize an instance to be returned to a pool.
        /// </summary>
        /// <param name="item"></param>
        /// <exception cref="PoolLifecycleException">if the item cannot be passivated</exception>
        void PassivateItem(TItem item);
    }
}
