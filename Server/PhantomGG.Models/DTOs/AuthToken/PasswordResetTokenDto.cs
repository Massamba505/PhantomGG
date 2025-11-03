namespace PhantomGG.Models.DTOs.AuthToken;

public class PasswordResetTokenDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}