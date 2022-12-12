using Taskmony.DTOs;
using Taskmony.Models;

namespace Taskmony.Services;

public interface IUserService
{
    Task<(string? error, User? user)> GetUserAsync(UserAuthRequest request);

    Task<string?> AddUserAsync(UserRegisterRequest request);

    string? ValidateUserCredentials(string login, string password);

    IQueryable<User> GetUsers(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit,
        Guid currentUserId);
}