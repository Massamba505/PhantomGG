namespace PhantomGG.API.DTOs.AuthToken;

public class AccessTokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
