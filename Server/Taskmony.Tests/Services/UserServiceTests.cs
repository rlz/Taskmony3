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

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();

        _userService = new UserService(_mockUserRepository.Object, new Mock<IPasswordHasher>().Object,
            new Mock<ITokenProvider>().Object);
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
    public async Task AddUserAsync_ThrowsWhenLoginIsInvalid()
    {
        var request = UserFixture.GetRegisterRequestWithInvalidLogin();

        var exception = await Assert.ThrowsAsync<DomainException>(() => _userService.AddUserAsync(request));
        Assert.Equivalent(ValidationErrors.InvalidLogin, exception.Error);
    }

    [Fact]
    public async Task AddUserAsync_ThrowsWhenEmailIsInvalid()
    {
        var request = UserFixture.GetRegisterRequestWithInvalidEmail();

        var exception = await Assert.ThrowsAsync<DomainException>(() => _userService.AddUserAsync(request));
        Assert.Equivalent(ValidationErrors.InvalidEmail, exception.Error);
    }

    [Fact]
    public async Task AddUserAsync_ThrowsWhenDisplayNameIsInvalid()
    {
        var request = UserFixture.GetRegisterRequestWithInvalidDisplayName();

        var exception = await Assert.ThrowsAsync<DomainException>(() => _userService.AddUserAsync(request));
        Assert.Equivalent(ValidationErrors.InvalidDisplayName, exception.Error);
    }

    [Theory]
    [InlineData("password")]
    [InlineData("pa55word")]
    [InlineData("Password")]
    [InlineData("Pa55d")]
    public async Task AddUserAsync_ThrowsWhenPasswordIsInvalid(string password)
    {
        var request = UserFixture.GetRegisterRequestWithPassword(password);

        var exception = await Assert.ThrowsAsync<DomainException>(() => _userService.AddUserAsync(request));
        Assert.Equivalent(ValidationErrors.InvalidPassword, exception.Error);
    }

    [Fact]
    public async Task SetDisplayNameAsync_ReturnsTrueWhenDisplayNameIsUpdated()
    {
        var userId = Guid.NewGuid();
        var displayName = "display name";

        _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(UserFixture.GetUserWithId(userId));
        _mockUserRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(true);

        var result = await _userService.SetDisplayNameAsync(userId, displayName, userId);

        Assert.True(result);
    }
}