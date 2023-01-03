using Taskmony.DTOs;
using Taskmony.Models;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services;

public interface IUserService
{
    Task<UserAuthResponse> AuthenticateAsync(UserAuthRequest request);

    Task AddAsync(UserRegisterRequest request);

    IQueryable<User> Get(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit,
        Guid currentUserId);
}