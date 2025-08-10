namespace PhantomGG.API.DTOs.User;

/// <summary>
/// DTO for user profile information
/// </summary>
public class UserProfileDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to profile picture
    /// </summary>
    public string? ProfilePictureUrl { get; set; }
    
    /// <summary>
    /// User role
    /// </summary>
    public string Role { get; set; } = string.Empty;
    
    /// <summary>
    /// Creation date
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
