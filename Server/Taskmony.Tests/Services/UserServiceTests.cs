using Moq;
using Taskmony.Auth;
using Taskmony.DTOs;
using Taskmony.Errors;
using Taskmony.Exceptions;
using Taskmony.Models.ValueObjects;
using Taskmony.Repositories.Abstract;
using Taskmony.Services;
using Taskmony.Services.Abstract;
using Taskmony.Tests.Fixtures;

namespace Taskmony.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly UserService _userService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly Mock<ITokenProvider> _mockTokenProvider;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _passwordHasher = new PasswordHasher();
        _mockTokenProvider = new Mock<ITokenProvider>();

        _userService = new UserService(_mockUserRepository.Object, _passwordHasher, _mockTokenProvider.Object);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsCurrentUserWhenFiltersAreEmpty()
    {
        var userId = Guid.NewGuid();
        var user = UserFixture.GetUser(userId);

        _mockUserRepository
            .Setup(r => r.GetAsync(new[] {userId}, null, null, null, null))
            .ReturnsAsync(new[] {user});

        var result = (await _userService.GetUsersAsync(null, null, null, null, null, userId)).ToList();

        Assert.Single(result);
        Assert.All(result, u => Assert.Null(u.Password));

        _mockUserRepository.Verify(r => r.GetAsync(new[] {userId}, null, null, null, null), Times.Once);
    }

    [Fact]
    public async Task GetUsersAsync_ReturnsEmailOfCurrentUserOnly()
    {
        var userId = Guid.NewGuid();
        var user = UserFixture.GetUser(userId);
        var users = new[] {user, UserFixture.GetUser(Guid.NewGuid()), UserFixture.GetUser(Guid.NewGuid())};

        _mockUserRepository
            .Setup(r => r.GetAsync(It.IsAny<Guid[]?>(), It.IsAny<string[]?>(), It.IsAny<string[]?>(), It.IsAny<int?>(),
                It.IsAny<int?>()))
            .ReturnsAsync(users);

        var result = (await _userService.GetUsersAsync(null, null, null, null, null, userId)).ToList();

        Assert.All(result, u =>
        {
            Assert.Null(u.Password);
            Assert.Equal(u.Id == userId ? u.Email : null, u.Email);
        });
    }

    [Theory]
    [InlineData("login", "Pa55word", "name", "a@a.a")]
    [InlineData("login", "P@55word!", "name", "a@a.a")]
    public async Task AddUserAsync_ReturnsTrueWhenUserIsAdded(string login, string password, string displayName,
        string email)
    {
        var request = new UserRegisterRequest(login, password, displayName, email);

        _mockUserRepository.Setup(r => r.AnyUserWithLoginAsync(It.IsAny<Login>())).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.AnyUserWithEmailAsync(It.IsAny<Email>())).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _userService.AddUserAsync(request);

        Assert.True(result);
    }

    [Fact]
    public async Task AddUserAsync_ReturnsFalseWhenUserIsNotAdded()
    {
        var request = UserFixture.GetValidRegisterRequest();

        _mockUserRepository.Setup(r => r.AnyUserWithLoginAsync(It.IsAny<Login>())).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.AnyUserWithEmailAsync(It.IsAny<Email>())).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(false);

        var result = await _userService.AddUserAsync(request);

        Assert.False(result);
    }

    [Fact]
    public async Task AddUserAsync_ThrowsWhenLoginIsAlreadyInUse()
    {
        var request = UserFixture.GetValidRegisterRequest();

        _mockUserRepository.Setup(r => r.AnyUserWithLoginAsync(It.IsAny<Login>())).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _userService.AddUserAsync(request));
        Assert.Equivalent(UserErrors.LoginIsAlreadyInUse, exception.Error);
    }

    [Fact]
    public async Task AddUserAsync_ThrowsWhenEmailIsAlreadyInUse()
    {
        var request = UserFixture.GetValidRegisterRequest();

        _mockUserRepository.Setup(r => r.AnyUserWithLoginAsync(It.IsAny<Login>())).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.AnyUserWithEmailAsync(It.IsAny<Email>())).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _userService.AddUserAsync(request));
        Assert.Equivalent(UserErrors.EmailIsAlreadyInUse, exception.Error);
    }

    [Fact]
    public async Task SetDisplayNameAsync()
    {
        var userId = Guid.NewGuid();
        var displayName = "display name";
        var user = UserFixture.GetUser(userId);

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _userService.SetDisplayNameAsync(userId, displayName, userId);

        Assert.True(result);
        Assert.NotNull(user.DisplayName);
        Assert.Equal(displayName, user.DisplayName.Value);

        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetLoginAsync()
    {
        var userId = Guid.NewGuid();
        var login = "login";
        var user = UserFixture.GetUser(userId);

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _userService.SetLoginAsync(userId, login, userId);

        Assert.True(result);
        Assert.NotNull(user.Login);
        Assert.Equal(login, user.Login.Value);

        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetEmailAsync()
    {
        var userId = Guid.NewGuid();
        var email = "new@a.a";
        var user = UserFixture.GetUser(userId);

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _userService.SetEmailAsync(userId, email, userId);

        Assert.True(result);
        Assert.NotNull(user.Email);
        Assert.Equal(email, user.Email.Value);

        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task SetPasswordAsync()
    {
        var userId = Guid.NewGuid();
        var password = "newPa55word";
        var oldPassword = "oldPa55word";
        var user = UserFixture.GetUser(userId);
        user.UpdatePassword(_passwordHasher.HashPassword(Password.From(oldPassword)));

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _userService.SetPasswordAsync(userId, oldPassword, password, userId);

        Assert.True(result);
        Assert.NotNull(user.Password);

        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockTokenProvider.Verify(p => p.RevokeUserRefreshTokens(userId), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("123456789012345678901234")]
    [InlineData("Password")]
    [InlineData("P@ssword")]
    [InlineData("P@ss0rd")]
    public async Task SetPasswordAsync_ThrowsWhenPasswordIsInvalid(string password)
    {
        var userId = Guid.NewGuid();
        var oldPassword = "oldPa55word";
        var user = UserFixture.GetUser(userId);
        user.UpdatePassword(_passwordHasher.HashPassword(Password.From(oldPassword)));

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _userService.SetPasswordAsync(userId, oldPassword, password, userId));

        Assert.Equivalent(ValidationErrors.InvalidPassword, exception.Error);
    }

    [Fact]
    public async Task SetPasswordAsync_ThrowsWhenOldPasswordIsIncorrect()
    {
        var userId = Guid.NewGuid();
        var password = "newPa55word";
        var oldPassword = "oldPa55word";
        var user = UserFixture.GetUser(userId);
        user.UpdatePassword(_passwordHasher.HashPassword(Password.From(oldPassword)));

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            _userService.SetPasswordAsync(userId, "oldP@55word", password, userId));

        Assert.Equivalent(UserErrors.WrongPassword, exception.Error);
    }

    [Fact]
    public async Task SetNotificationReadTimeAsync()
    {
        var userId = Guid.NewGuid();
        var user = UserFixture.GetUser(userId);

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _userService.SetNotificationReadTimeAsync(userId, DateTime.UtcNow, userId);

        Assert.True(result);
        Assert.NotNull(user.NotificationReadTime);

        _mockUserRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}