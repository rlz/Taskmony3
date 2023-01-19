namespace Taskmony.Services;

public interface IUserIdentifierProvider
{
    Guid UserId { get; }
}