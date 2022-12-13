using Taskmony.Models;

namespace Taskmony.Auth;

public interface IJwtProvider
{
    string GenerateToken(User user);
}