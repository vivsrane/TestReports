namespace VB.Common.Core.Membership
{
    public interface IMember
    {
        int Id { get; }
        string UserName { get; }
        string FirstName { get; }
        string LastName { get; }
    }
}