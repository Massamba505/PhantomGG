namespace PhantomGG.API.DTOs.User;

/// <summary>
/// Basic user information DTO
/// </summary>
public class UserDto
{
    /// <summary>
    /// User ID
    /// </summary>
    public string Id { get; set; } = string.Empty;
    
    /// <summary>
    /// Email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// First name
    /// </summary>
    public string FirstName { get; set; } = string.Empty;
    
    /// <summary>
    /// Last name
    /// </summary>
    public string LastName { get; set; } = string.Empty;
    
    /// <summary>
    /// URL to profile picture
    /// </summary>
    public string? ProfilePicture { get; set; }
}
