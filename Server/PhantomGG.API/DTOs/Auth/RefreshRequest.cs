using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = null!;
}
