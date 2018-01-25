namespace VB.Common.Core.Memento
{
    public interface IOriginator<TEntity>
    {
        IMemento<TEntity> ToMemento();
    }
}