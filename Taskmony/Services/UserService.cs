using Taskmony.DTOs;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;

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

    public async Task<bool> AddUserAsync(UserRegisterRequest request)
    {
        var user = new User
        {
            Login = Login.From(request.Login),
            Email = Email.From(request.Email),
            DisplayName = DisplayName.From(request.DisplayName),
        };

        var password = Password.From(request.Password);

        if (await _userRepository.AnyUserWithLoginAsync(user.Login))
        {
            throw new DomainException(UserErrors.LoginIsAlreadyInUse);
        }

        if (await _userRepository.AnyUserWithEmailAsync(user.Email))
        {
            throw new DomainException(UserErrors.EmailIsAlreadyInUse);
        }

        user.Password = _passwordHasher.HashPassword(password);

        await _userRepository.AddAsync(user);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetUsersAsync(Guid[]? id, string[]? email, string[]? login,
        int? offset, int? limit, Guid currentUserId)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

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

        var users = (await _userRepository.GetAsync(id, email, login, offsetValue, limitValue)).ToList();

        return users.Select(u => new User
        {
            Id = u.Id,
            Email = currentUserId == u.Id ? u.Email : null,
            Login = u.Login,
            DisplayName = u.DisplayName,
            NotificationReadTime = u.Id == currentUserId ? u.NotificationReadTime : null,
        });
    }

    public async Task<bool> SetEmailAsync(Guid id, string email, Guid currentUserId)
    {
        var emailValue = Email.From(email);
        var user = await GetUserOrThrowAsync(id);

        user.Email = emailValue;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetLoginAsync(Guid id, string login, Guid currentUserId)
    {
        var loginValue = Login.From(login);
        var user = await GetUserOrThrowAsync(id);

        user.Login = loginValue;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDisplayNameAsync(Guid id, string displayName, Guid currentUserId)
    {
        var displayNameValue = DisplayName.From(displayName);
        var user = await GetUserOrThrowAsync(id);

        user.DisplayName = displayNameValue;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetNotificationReadTimeAsync(Guid id, DateTime notificationReadTime, Guid currentUserId)
    {
        if (notificationReadTime > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidNotificationReadTime);
        }

        var user = await GetUserOrThrowAsync(id);

        user.NotificationReadTime = notificationReadTime;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetPasswordAsync(Guid id, string oldPassword, string newPassword, Guid currentUserId)
    {
        var newPasswordValue = Password.From(newPassword);
        var user = await GetUserOrThrowAsync(id);

        if (!_passwordHasher.VerifyPassword(oldPassword, user.Password!))
        {
            throw new DomainException(UserErrors.WrongPassword);
        }

        user.Password = _passwordHasher.HashPassword(newPasswordValue);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<User> GetUserOrThrowAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user is null)
        {
            throw new DomainException(UserErrors.NotFound);
        }

        return user;
    }
}