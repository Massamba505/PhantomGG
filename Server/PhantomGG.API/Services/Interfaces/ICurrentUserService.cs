namespace PhantomGG.API.Services.Interfaces;

/// <summary>
/// Service for accessing the current user's information
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the current user's ID
    /// </summary>
    Guid? UserId { get; }
    
    /// <summary>
    /// Gets the current user's email
    /// </summary>
    string? Email { get; }
    
    /// <summary>
    /// Gets the current user's role
    /// </summary>
    string? Role { get; }
    
    /// <summary>
    /// Gets whether the current user is authenticated
    /// </summary>
    bool IsAuthenticated { get; }
}