using Taskmony.Auth;
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
    private readonly IDirectionRepository _directionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IUserRepository userRepository, IDirectionRepository directionRepository,
        IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _directionRepository = directionRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
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

        await _userRepository.AddUserAsync(user);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<UserAuthResponse> AuthenticateUserAsync(UserAuthRequest request)
    {
        var login = Login.From(request.Login);
        var password = Password.From(request.Password);

        var user = await _userRepository.GetUserByLoginAsync(login);

        if (user is null || !_passwordHasher.VerifyPassword(password.Value, user.Password!))
        {
            throw new DomainException(UserErrors.WrongLoginOrPassword);
        }

        var token = _jwtProvider.GenerateToken(user);

        return new UserAuthResponse(user.Id, user.DisplayName!.Value, token);
    }

    public async Task<IEnumerable<User>> GetUsersAsync(Guid[]? id, string[]? email, string[]? login,
        int? offset, int? limit, Guid currentUserId)
    {
        int? limitValue = limit is null ? null : Limit.From(limit.Value).Value;
        int? offsetValue = offset is null ? null : Offset.From(offset.Value).Value;

        if (id is null && email is null && login is null)
        {
            id = new[] { currentUserId };
        }

        email = email?
            .Where(e => !string.IsNullOrWhiteSpace(e))
            .Select(e => e.Trim())
            .ToArray();

        login = login?
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Trim())
            .ToArray();

        var users = (await _userRepository.GetUsersAsync(id, email, login, offsetValue, limitValue)).ToList();
        var idsOfUsersThatCanBeSeen = await GetIdsOfUsersThatCanBeSeen(currentUserId, users.Select(u => u.Id));

        return users.Select(x => new User
        {
            Id = x.Id,
            Email = idsOfUsersThatCanBeSeen.Contains(x.Id) ? x.Email : null,
            Login = x.Login,
            DisplayName = x.DisplayName,
            NotificationReadTime = x.Id == currentUserId ? x.NotificationReadTime : null,
        });
    }

    private async Task<IEnumerable<Guid>> GetIdsOfUsersThatCanBeSeen(Guid seerId, IEnumerable<Guid> seenIds)
    {
        return await _directionRepository.GetIdsOfUsersWithCommonDirection(seerId, seenIds);
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

    public async Task<bool> SetPasswordAsync(Guid id, string password, Guid currentUserId)
    {
        var passwordValue = Password.From(password);
        var user = await GetUserOrThrowAsync(id);

        user.Password = _passwordHasher.HashPassword(passwordValue);

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<User> GetUserOrThrowAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);

        if (user is null)
        {
            throw new DomainException(UserErrors.NotFound);
        }

        return user;
    }
}