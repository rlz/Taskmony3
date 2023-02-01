using Taskmony.ValueObjects;

namespace Taskmony.Services;

public interface IPasswordHasher
{
    string HashPassword(Password password);

    bool VerifyPassword(string password, string hash);
}