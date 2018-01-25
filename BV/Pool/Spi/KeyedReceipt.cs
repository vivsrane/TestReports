namespace VB.Common.Pool.Spi
{
    public class KeyedReceipt<TItem,TKey> : Receipt<TItem>
    {
        private readonly TKey key;

        public KeyedReceipt(TKey key, TItem item) : base(item)
        {
            this.key = key;
        }

        public TKey Key
        {
            get { return key; }
        }
    }
}
