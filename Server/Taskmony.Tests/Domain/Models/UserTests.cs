using Taskmony.Exceptions;
using Taskmony.Models.Users;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Tests.Domain.Models;

public class UserTests
{
    [Fact]
    public void CreateUser()
    {
        var user = GetValidUser();

        Assert.Equal(Login.From("login"), user.Login);
        Assert.Equal(DisplayName.From("display name"), user.DisplayName);
        Assert.Equal(Email.From("a@a.a"), user.Email);
        Assert.Equal("password hash", user.Password);
        Assert.Null(user.NotificationReadTime);
    }
    
    [Theory]
    [InlineData("login")]
    [InlineData("log1n")]
    [InlineData("logIn")]
    public void UpdateLogin(string login)
    {
        var user = GetValidUser();

        user.UpdateLogin(Login.From(login));

        Assert.Equal(Login.From(login), user.Login);
    }
    
    [Theory]
    [InlineData("new display name")]
    [InlineData("displayName")]
    [InlineData("d!splay name")]
    public void UpdateDisplayName(string name)
    {
        var user = GetValidUser();
        var displayName = DisplayName.From(name);

        user.UpdateDisplayName(displayName);

        Assert.Equal(displayName, user.DisplayName);
    }
    
    [Fact]
    public void UpdateEmail()
    {
        var user = GetValidUser();
        var email = Email.From("b@b.b");
        
        user.UpdateEmail(email);
        
        Assert.Equal(email, user.Email);
    }
    
    [Fact]
    public void UpdatePassword()
    {
        var user = GetValidUser();
        var password = "new password hash";
        
        user.UpdatePassword(password);
        
        Assert.Equal(password, user.Password);
    }
    
    [Fact]
    public void UpdateNotificationReadTime()
    {
        var user = GetValidUser();
        var notificationReadTime = DateTime.UtcNow;
        
        user.UpdateNotificationReadTime(notificationReadTime);
        
        Assert.Equal(notificationReadTime, user.NotificationReadTime);
    }
    
    [Fact]
    public void UpdateNotificationReadTime_ThrowsWhenInvalid()
    {
        var user = GetValidUser();
        var notificationReadTime = DateTime.UtcNow.AddDays(1);
        
        Assert.Throws<DomainException>(() => user.UpdateNotificationReadTime(notificationReadTime));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("     ")]
    [InlineData(null)]
    [InlineData("i")]
    [InlineData("in")]
    [InlineData("inv")]
    [InlineData("in va lid")]
    [InlineData("1inval!d")]
    public void UpdateLogin_ThrowsWhenInvalid(string login)
    {
        var user = GetValidUser();
        
        Assert.Throws<DomainException>(() => user.UpdateLogin(Login.From(login)));
        Assert.Throws<DomainException>(() => user.UpdateLogin(Login.From(new string('a', 51))));
    }
    
    [Theory]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("i")]
    [InlineData("in")]
    [InlineData(null)]
    public void UpdateDisplayName_ThrowsWhenInvalid(string? displayName)
    {
        var user = GetValidUser();
        
        Assert.Throws<DomainException>(() => user.UpdateDisplayName(DisplayName.From(displayName!)));
        Assert.Throws<DomainException>(() => user.UpdateDisplayName(DisplayName.From(new string('a', 101))));
    }

    private User GetValidUser()
    {
        return new User(
            Login.From("login"),
            DisplayName.From("display name"),
            Email.From("a@a.a"),
            "password hash");
    }
}