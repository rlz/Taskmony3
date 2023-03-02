namespace Taskmony.Auth;

public class JwtOptions
{
    public string Secret { get; set; } = default!;

    public string Issuer { get; set; } = default!;

    public string Audience { get; set; } = default!;
    
    public int AccessTokenExpirationMinutes { get; set; }

    public int RefreshTokenExpirationMinutes { get; set; }
}