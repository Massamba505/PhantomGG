namespace PhantomGG.API.DTOs.Auth;

public class AuthResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpires { get; set; }
    public DateTime? RefreshTokenExpires { get; set; }
    public UserDto? User { get; set; }
}

public class TokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpires { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<string> Roles { get; set; } = new List<string>();
}
