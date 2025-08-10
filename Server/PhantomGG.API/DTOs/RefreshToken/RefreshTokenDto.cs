namespace PhantomGG.API.DTOs.RefreshToken;

public class RefreshTokenDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? RevokedAt { get; set; }
    public bool IsRevoked { get; set; }
}
