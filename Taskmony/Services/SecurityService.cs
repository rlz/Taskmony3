using Taskmony.Auth;
using Taskmony.DTOs;
using Taskmony.Emails;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models;
using Taskmony.Repositories.Abstract;
using Taskmony.Services.Abstract;
using Taskmony.ValueObjects;
using Task = System.Threading.Tasks.Task;

namespace Taskmony.Services;

public class SecurityService : ISecurityService
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserIdentifierProvider _userIdentifierProvider;
    private readonly IEmailService _emailService;
    private readonly IVerificationTokenRepository _verificationTokenRepository;

    public SecurityService(ITokenProvider tokenProvider, IUserRepository userRepository,
        IPasswordHasher passwordHasher, IRefreshTokenRepository refreshTokenRepository,
        IUserIdentifierProvider userIdentifierProvider, IEmailService emailService,
        IVerificationTokenRepository verificationTokenRepository)
    {
        _tokenProvider = tokenProvider;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _refreshTokenRepository = refreshTokenRepository;
        _userIdentifierProvider = userIdentifierProvider;
        _emailService = emailService;
        _verificationTokenRepository = verificationTokenRepository;
    }

    public async Task<UserAuthResponse> AuthenticateAsync(UserAuthRequest request)
    {
        var login = Login.From(request.Login);
        var password = Password.From(request.Password);

        var user = await _userRepository.GetByLoginAsync(login);

        if (user is null || !_passwordHasher.VerifyPassword(password.Value, user.Password!))
        {
            throw new DomainException(UserErrors.WrongLoginOrPassword);
        }

        if (!user.IsActive)
        {
            throw new DomainException(UserErrors.UserIsNotActive);
        }

        var (accessToken, refreshToken) = await _tokenProvider.GenerateTokensAsync(user);

        return new UserAuthResponse(user.Id, user.DisplayName!.Value, accessToken, refreshToken);
    }

    public async Task<RefreshTokenResponse> RefreshTokensAsync(RefreshTokenRequest request)
    {
        var (accessToken, refreshToken) = await _tokenProvider.RefreshTokensAsync(request.RefreshToken);

        return new RefreshTokenResponse(accessToken, refreshToken);
    }

    public async Task<bool> RevokeAllUserTokensAsync()
    {
        var tokens = await _refreshTokenRepository.GetByUserIdAsync(_userIdentifierProvider.UserId);

        tokens.ToList().ForEach(t => t.IsRevoked = true);

        return await _refreshTokenRepository.SaveChangesAsync();
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken)
    {
        var token = await _refreshTokenRepository.GetAsync(refreshToken);

        if (token is null)
        {
            throw new DomainException(TokenErrors.InvalidToken);
        }

        token.IsRevoked = true;

        return await _refreshTokenRepository.SaveChangesAsync();
    }

    public async Task SendConfirmationEmailAsync(User user, Uri baseUri)
    {
        var token = Guid.NewGuid();

        _verificationTokenRepository.Add(new VerificationToken
        {
            UserId = user.Id,
            Token = token
        });

        await _verificationTokenRepository.SaveChangesAsync();

        var confirmUrl = new Uri(baseUri, $"api/account/confirm-email?userId={user.Id}&token={token}");

        // TODO: create pretty template
        await _emailService.SendEmailAsync(user.Email!.Value, "Verify your email address",
            $"To complete your registration, please verify your email address: <a href='{confirmUrl}'>verify email</a>");
    }

    public async Task ConfirmEmailAsync(Guid userId, Guid token)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            throw new DomainException(UserErrors.WrongConrimationLink);
        }

        if (user.IsActive)
        {
            return;
        }

        var verificationToken = await _verificationTokenRepository.GetAsync(userId, token);

        if (verificationToken is null)
        {
            throw new DomainException(UserErrors.WrongConrimationLink);
        }

        user.IsActive = true;

        await _userRepository.SaveChangesAsync();

        _verificationTokenRepository.DeleteByUserId(userId);

        await _verificationTokenRepository.SaveChangesAsync();
    }
}