namespace VB.Common.Core.Data
{
    public interface IDataSessionManager
    {
        IDataSession CreateSession(string databaseName);

        IDataSession Session { get; }
    }
}