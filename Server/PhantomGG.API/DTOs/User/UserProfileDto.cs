namespace PhantomGG.API.DTOs.User;

public class UserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;  
    public string Email { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
