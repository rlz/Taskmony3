namespace Taskmony.DTOs;

public record UserRegisterRequest(string Login, string Password, string DisplayName, string Email);