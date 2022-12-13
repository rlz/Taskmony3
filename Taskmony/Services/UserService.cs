using System.Text.RegularExpressions;
using Taskmony.DTOs;
using Taskmony.Models;
using Taskmony.Repositories;

namespace Taskmony.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<string?> AddUserAsync(UserRegisterRequest request)
    {
        var error = ValidateCredentials(request.Login, request.Password, request.Email);

        if (error is not null)
        {
            return error;
        }

        try
        {
            return await _userRepository.AddUserAsync(new User
            {
                Login = request.Login,
                Password = _passwordHasher.HashPassword(request.Password),
                Email = request.Email,
                DisplayName = request.DisplayName
            });
        }
        catch (Exception)
        {
            return "Failed to add user";
        }
    }

    public async Task<(string? error, User? user)> GetUserAsync(UserAuthRequest request)
    {
        var error = ValidateCredentials(request.Login, request.Password);

        if (error is not null)
        {
            return (error, null);
        }

        try
        {
            var user = await _userRepository.GetUserAsync(request.Login);

            if (user is null)
            {
                return ("User not found", null);
            }

            if (!_passwordHasher.VerifyPassword(request.Password, user.Password!))
            {
                return ("Wrong password", null);
            }

            return (null, await _userRepository.GetUserAsync(request.Login));
        }
        catch
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

    public string? ValidateCredentials(string login, string password)
    {
        if (login is null || login.Length < 4)
        {
            return "Login must contain at least 4 characters";
        }

        if (!login.All(c => char.IsLetterOrDigit(c)))
        {
            return "Login must contain only letters and digits";
        }

        if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,128}$"))
        {
            return "Password must contain minimum eight characters, at least " +
                   "one uppercase letter, one lowercase letter and one number";
        }

        return null;
    }

    public string? ValidateCredentials(string login, string password, string email)
    {
        if (email is null || email.Length < 4)
        {
            return "Email must contain at least 4 characters";
        }

        try
        {
            _ = new System.Net.Mail.MailAddress(email);
        }
        catch
        {
            return "Invalid email format";
        }

        return ValidateCredentials(login, password);
    }
}