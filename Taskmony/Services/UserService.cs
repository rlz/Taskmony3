using System.Text.RegularExpressions;
using Taskmony.DTOs;
using Taskmony.Models;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<string?> AddUserAsync(UserRegisterRequest request)
    {
        var error = ValidateUserCredentials(request.Login, request.Password);

        if (error is not null)
        {
            return error;
        }

        try
        {
            error = await _userRepository.AddUserAsync(new User
            {
                Login = request.Login,
                Password = request.Password,
                Email = request.Email,
                DisplayName = request.DisplayName
            });
        }
        catch (Exception)
        {
            return "Failed to add user";
        }

        return error;
    }

    public async Task<(string? error, User? user)> GetUserAsync(UserAuthRequest request)
    {
        try
        {
            return (null, await _userRepository.GetUserAsync(request));
        }
        catch (Exception)
        {
            return ("Failed to get user", null);
        }
    }

    public IQueryable<User> GetUsers(Guid[]? id, string[]? email, string[]? login, int? offset, int? limit,
        Guid currentUserId)
    {
        if (id is null && email is null && login is null)
        {
            id = new[] {currentUserId};
        }

        email = email?
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e.Trim())
            .ToArray();

        login = login?
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Trim())
            .ToArray();

        return _userRepository.GetUsers(id, email, login, offset, limit);
    }

    public string? ValidateUserCredentials(string login, string password)
    {
        if (login is null || login.Length < 4)
        {
            return "Login must contain at least 4 characters";
        }

        if (!login.All(c => char.IsLetterOrDigit(c)))
        {
            return "Login must contain only letters and digits";
        }

        if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,30}$"))
        {
            return "Password must contain at least 8 characters and at least 1 digit";
        }

        return null;
    }
}