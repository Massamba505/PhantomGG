namespace PhantomGG.Models.DTOs.AuthToken;

public class EmailVerificationTokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}