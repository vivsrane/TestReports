namespace VB.Common.Core.Validation
{
    public interface IConstraint<in T>
    {
        bool IsSatisfiedBy(T value);

        string ResourceKey { get; }

        bool StopProcessing { get; }
    }
}