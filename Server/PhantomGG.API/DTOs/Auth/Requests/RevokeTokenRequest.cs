using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth.Requests;

public class RevokeTokenRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
