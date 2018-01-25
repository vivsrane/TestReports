namespace VB.Common.Core.Memento
{
    public interface IMementoFactory<T>
    {
        IMemento<T> For(T item);
    }
}