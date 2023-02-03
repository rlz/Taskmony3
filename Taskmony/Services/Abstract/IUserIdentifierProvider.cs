namespace Taskmony.Services.Abstract;

public interface IUserIdentifierProvider
{
    Guid UserId { get; }
}