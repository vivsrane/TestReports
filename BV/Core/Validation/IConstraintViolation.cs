namespace VB.Common.Core.Validation
{
    public interface IConstraintViolation
    {
        object[] Arguments { get; }

        string ResourceKey { get; }
    }
}