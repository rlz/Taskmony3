using Taskmony.Auth;
using Taskmony.DTOs;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.Users;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;

namespace Taskmony.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenProvider _tokenProvider;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher,
        ITokenProvider tokenProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _tokenProvider = tokenProvider;
    }

    public async Task<bool> AddUserAsync(UserRegisterRequest request)
    {
        var login = Login.From(request.Login.Trim());
        var email = Email.From(request.Email.Trim());
        var displayName = DisplayName.From(request.DisplayName.Trim());
        var password = Password.From(request.Password.Trim());

        if (await _userRepository.AnyUserWithLoginAsync(login))
        {
            throw new DomainException(UserErrors.LoginIsAlreadyInUse);
        }

        if (await _userRepository.AnyUserWithEmailAsync(email))
        {
            throw new DomainException(UserErrors.EmailIsAlreadyInUse);
        }

        var passwordHash = _passwordHasher.HashPassword(password);

        await _userRepository.AddAsync(new User(login, displayName, email, passwordHash));

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<User>> GetUsersAsync(Guid[]? id, string[]? email, string[]? login,
        int? offset, int? limit, Guid currentUserId)
    {
        int? limitValue = limit == null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset == null ? null : Offset.From(offset.Value).Value;

        if (id == null && email == null && login == null)
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

        return users.Select(u => new User(
            id: u.Id,
            login: u.Login!,
            email: currentUserId == u.Id ? u.Email : null,
            displayName: u.DisplayName!,
            notificationReadTime: u.Id == currentUserId ? u.NotificationReadTime : null));
    }

    public async Task<bool> SetEmailAsync(Guid id, string email, Guid currentUserId)
    {
        var newEmail = Email.From(email.Trim());
        var user = await GetUserOrThrowAsync(id);

        user.UpdateEmail(newEmail);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetLoginAsync(Guid id, string login, Guid currentUserId)
    {
        var newLogin = Login.From(login.Trim());
        var user = await GetUserOrThrowAsync(id);

        user.UpdateLogin(newLogin);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDisplayNameAsync(Guid id, string displayName, Guid currentUserId)
    {
        var newDisplayName = DisplayName.From(displayName.Trim());
        var user = await GetUserOrThrowAsync(id);

        user.UpdateDisplayName(newDisplayName);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetNotificationReadTimeAsync(Guid id, DateTime notificationReadTime, Guid currentUserId)
    {
        var user = await GetUserOrThrowAsync(id);

        user.UpdateNotificationReadTime(notificationReadTime);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetPasswordAsync(Guid id, string oldPassword, string newPassword, Guid currentUserId)
    {
        var newPasswordValue = Password.From(newPassword);
        var oldPasswordValue = Password.From(oldPassword);
        var user = await GetUserOrThrowAsync(id);

        if (!_passwordHasher.VerifyPassword(oldPasswordValue.Value, user.Password!))
        {
            throw new DomainException(UserErrors.WrongPassword);
        }

        user.UpdatePassword(_passwordHasher.HashPassword(newPasswordValue));

        if (!await _userRepository.SaveChangesAsync())
        {
            return false;
        }

        return await _tokenProvider.RevokeUserRefreshTokens(id);
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