namespace VB.Common.Core.DTO
{
    public interface ITransferObject<T> 
    {
        T ToTransferObject();
    }
}