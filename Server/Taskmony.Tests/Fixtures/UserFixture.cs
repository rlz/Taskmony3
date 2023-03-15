using Taskmony.DTOs;
using Taskmony.Models;

namespace Taskmony.Tests.Fixtures;

public static class UserFixture
{
    public static UserRegisterRequest GetValidRegisterRequest() =>
        new UserRegisterRequest("login", "Pa55word", "name", "a@a.a");

    public static UserRegisterRequest GetRegisterRequestWithInvalidEmail() =>
        new UserRegisterRequest("login", "Pa55word", "name", "a");

    public static UserRegisterRequest GetRegisterRequestWithPassword(string password) =>
        new UserRegisterRequest("login", password, "name", "a@a.a");

    public static UserRegisterRequest GetRegisterRequestWithInvalidLogin() =>
        new UserRegisterRequest("l", "Pa55word", "name", "a@a.a");
    
    public static UserRegisterRequest GetRegisterRequestWithInvalidDisplayName() =>
        new UserRegisterRequest("login", "Pa55word", "n", "a@a");

    public static User GetUserWithId(Guid otherUserId)
    {
        return new User
        {
            Id = otherUserId,
        };
    }
}