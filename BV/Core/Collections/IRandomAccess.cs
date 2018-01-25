using System.Collections.Generic;

namespace VB.Common.Core.Collections
{
    public interface IRandomAccess<T> : IEnumerable<T>
    {
        void Add(T item);

        int Count { get; }

        void Freeze();

        T this[int index] { get; }
    }
}
