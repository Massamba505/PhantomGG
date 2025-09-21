using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PhantomGG.Models.DTOs.Player;

public class CreatePlayerDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(30)]
    public string? Position { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    public IFormFile? PhotoUrl { get; set; }

    [Required]
    public Guid TeamId { get; set; }
}
