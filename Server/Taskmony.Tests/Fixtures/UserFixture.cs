using Taskmony.DTOs;
using Taskmony.Models.Users;
using Taskmony.Models.ValueObjects;

namespace Taskmony.Tests.Fixtures;

public static class UserFixture
{
    public static UserRegisterRequest GetValidRegisterRequest() =>
        new UserRegisterRequest("login", "Pa55word", "name", "a@a.a");

    public static User GetUser(Guid userId)
    {
        return new User(userId, Login.From("login"), DisplayName.From("display name"), Email.From("a@a.a"), null);
    }
}