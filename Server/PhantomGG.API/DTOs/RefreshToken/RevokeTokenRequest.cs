using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.RefreshToken;

public class RevokeTokenRequest
{
    [Required]
    public Guid RefreshToken { get; set; }
}
