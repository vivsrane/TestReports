namespace VB.Common.Core.Command
{
    public interface ICommandParameter<TParameter>
    {
        TParameter Parameter { get; }
    }
}