namespace Taskmony.DTOs;

public class UserAuthResponse
{
    public Guid Id { get; set; }

    public string DisplayName { get; set; }

    public string AccessToken { get; set; }
}