using System;

namespace VB.Common.Pool
{
    public interface IKeyedPool<TItem,TKey>
    {
        string InstanceName
        {
            get;
        }

        IKeyedPooledItemFactory<TItem, TKey> Factory
        {
            get;
        }

        TItem BorrowItem(TKey key);

        TItem BorrowItem(TKey key, TimeSpan timeout);

        void ReturnItem(TItem item);

        void Clear();

        void Close();
    }
}
