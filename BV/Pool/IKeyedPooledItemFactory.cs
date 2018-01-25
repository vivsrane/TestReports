namespace VB.Common.Pool
{
    public interface IKeyedPooledItemFactory<TItem,TKey>
    {
        TItem MakeItem(TKey key);

        void DestroyItem(TKey key, TItem item);

        bool ValidateItem(TKey key, TItem item);

        void ActivateItem(TKey key, TItem item);

        void PassivateItem(TKey key, TItem item);
    }
}
