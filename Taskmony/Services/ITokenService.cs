using Taskmony.DTOs;
using Taskmony.Models;

namespace Taskmony.Services;

public interface ITokenService
{
    (string? error, UserAuthResponse response) CreateToken(User user);
}