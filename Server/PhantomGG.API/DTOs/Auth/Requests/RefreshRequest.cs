using System.ComponentModel.DataAnnotations;

namespace PhantomGG.API.DTOs.Auth.Requests;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
