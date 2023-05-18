using Taskmony.Models.ValueObjects;

namespace Taskmony.Services.Abstract;

public interface IPasswordHasher
{
    string HashPassword(Password password);

    bool VerifyPassword(string password, string hash);
}