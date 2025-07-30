using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.RefreshToken;

public class RefreshTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}
