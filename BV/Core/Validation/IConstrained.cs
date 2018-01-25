namespace VB.Common.Core.Validation
{
    public interface IConstrained
    {
        bool IsValid { get; }

        IConstraintViolations ConstraintViolations { get; }
    }
}