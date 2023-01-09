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

    public async Task AddAsync(UserRegisterRequest request)
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

    public async Task<UserAuthResponse> AuthenticateAsync(UserAuthRequest request)
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

        var users = await _userRepository.GetUsersAsync(id, email, login, offset, limit);

        return users.Select(x => new User
        {
            Id = x.Id,
            Email = x.Id == currentUserId ? x.Email : null,
            Login = x.Id == currentUserId ? x.Login : null,
            DisplayName = x.DisplayName
        });
    }

    public void ValidateCredentials(string login, string password)
    {
        if (login is null || login.Length < 4 || !login.All(c => char.IsLetterOrDigit(c)))
        {
            throw new DomainException(UserErrors.InvalidLoginFormat);
        }

        //8+ characters, at least one uppercase letter, one lowercase letter and one number
        if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,128}$"))
        {
            throw new DomainException(UserErrors.InvalidPasswordFormat);
        }
    }

    private void ValidateCredentials(string login, string password, string email)
    {
        try
        {
            _ = new System.Net.Mail.MailAddress(email);
        }
        catch
        {
            throw new DomainException(UserErrors.InvalidEmailFormat);
        }

        ValidateCredentials(login, password);
    }
}