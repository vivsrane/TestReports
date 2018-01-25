namespace FirstLook.Common.Core
{
    public interface ICloneable<T>
    {
        /// <summary>
        /// Deep copy only
        /// </summary>
        /// <returns></returns>
        T Clone();
    }
}