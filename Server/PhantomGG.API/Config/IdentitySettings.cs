using Microsoft.AspNetCore.Identity;

namespace PhantomGG.API.Config;

public class IdentitySettings
{
    public PasswordOptions Password { get; set; } = new();
    public LockoutOptions Lockout { get; set; } = new();
    public UserOptions User { get; set; } = new();
}
