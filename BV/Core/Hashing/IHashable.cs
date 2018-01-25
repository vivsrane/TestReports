namespace VB.Common.Core.Hashing
{
    /// <summary>
    /// Interface for hashable objects.
    /// </summary>
    public interface IHashable
    {
        /// <summary>
        /// Create a hash value.
        /// </summary>
        /// <returns>Hash value.</returns>
        long Hash();
    }
}
