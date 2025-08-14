namespace PhantomGG.API.Common;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; set; }
    public int RefreshTokenExpiryDays { get; set; }
}

public class PasswordSettings
{
    public bool RequireDigit { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireNonAlphanumeric { get; set; }
    public int MinimumLength { get; set; }
    public int MaxFailedAttempts { get; set; }
    public int LockoutDurationMinutes { get; set; }
}

public enum UserRole
{
    Admin,
    Organizer,
    User
}

public static class Roles
{
    public const string Admin = "Admin";
    public const string Organizer = "Organizer";
    public const string User = "User";
}
