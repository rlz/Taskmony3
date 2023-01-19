using System.Text.RegularExpressions;
using Taskmony.Auth;
using Taskmony.DTOs;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Repositories;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtProvider _jwtProvider;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtProvider = jwtProvider;
    }

    public async Task AddUserAsync(UserRegisterRequest request)
    {
        ValidateCredentials(request.Login, request.Password, request.Email);

        if (await _userRepository.AnyUserWithLoginAsync(request.Login))
        {
            throw new DomainException(UserErrors.LoginIsAlreadyInUse);
        }

        if (await _userRepository.AnyUserWithEmailAsync(request.Email))
        {
            throw new DomainException(UserErrors.EmailIsAlreadyInUse);
        }

        await _userRepository.AddUserAsync(new User
        {
            Login = request.Login,
            Password = _passwordHasher.HashPassword(request.Password),
            Email = request.Email,
            DisplayName = request.DisplayName
        });

        await _userRepository.SaveChangesAsync();
    }

    public async Task<UserAuthResponse> AuthenticateUserAsync(UserAuthRequest request)
    {
        ValidateCredentials(request.Login, request.Password);

        var user = await _userRepository.GetUserByLoginAsync(request.Login);

        if (user is null)
        {
            throw new DomainException(UserErrors.WrongLoginOrPassword);
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.Password!))
        {
            throw new DomainException(UserErrors.WrongLoginOrPassword);
        }

        var token = _jwtProvider.GenerateToken(user);

        return new UserAuthResponse(user.Id, user.DisplayName!, token);
    }

    public async Task<IEnumerable<User>> GetUsersAsync(Guid[]? id, string[]? email, string[]? login,
        int? offset, int? limit, Guid currentUserId)
    {
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

        var users = await _userRepository.GetUsersAsync(id, email, login, offset, limit);

        return users.Select(x => new User
        {
            Id = x.Id,
            Email = x.Id == currentUserId ? x.Email : null,
            Login = x.Login,
            DisplayName = x.DisplayName
        });
    }

    public async Task<bool> SetEmail(Guid id, string email, Guid currentUserId)
    {
        var user = await GetUserOrThrow(id);

        ValidateEmail(email);

        user.Email = email;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetLogin(Guid id, string login, Guid currentUserId)
    {
        var user = await GetUserOrThrow(id);

        ValidateLogin(login);

        user.Login = login;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetDisplayName(Guid id, string displayName, Guid currentUserId)
    {
        var user = await GetUserOrThrow(id);

        user.DisplayName = displayName;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetNotificationReadTime(Guid id, DateTime notificationReadTime, Guid currentUserId)
    {
        var user = await GetUserOrThrow(id);

        if (notificationReadTime > DateTime.UtcNow)
        {
            throw new DomainException(ValidationErrors.InvalidNotificationReadTime);
        }

        user.NotificationReadTime = notificationReadTime;

        return await _userRepository.SaveChangesAsync();
    }

    public async Task<bool> SetPassword(Guid id, string password, Guid currentUserId)
    {
        var user = await GetUserOrThrow(id);

        ValidatePassword(password);

        user.Password = _passwordHasher.HashPassword(password);

        return await _userRepository.SaveChangesAsync();
    }

    private async Task<User> GetUserOrThrow(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);

        if (user is null)
        {
            throw new DomainException(UserErrors.NotFound);
        }

        return user;
    }

    public void ValidateLogin(string login)
    {
        if (login is null || login.Length < 4 || !login.All(c => char.IsLetterOrDigit(c)))
        {
            throw new DomainException(UserErrors.InvalidLoginFormat);
        }
    }

    public void ValidatePassword(string password)
    {
        //8+ characters, at least one uppercase letter, one lowercase letter and one number
        if (password is null || !Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,128}$"))
        {
            throw new DomainException(UserErrors.InvalidPasswordFormat);
        }
    }

    public void ValidateEmail(string email)
    {
        try
        {
            _ = new System.Net.Mail.MailAddress(email);
        }
        catch
        {
            throw new DomainException(UserErrors.InvalidEmailFormat);
        }
    }

    public void ValidateCredentials(string login, string password)
    {
        ValidateLogin(login);
        ValidatePassword(password);
    }

    private void ValidateCredentials(string login, string password, string email)
    {
        ValidateCredentials(login, password);
        ValidateEmail(email);
    }
}