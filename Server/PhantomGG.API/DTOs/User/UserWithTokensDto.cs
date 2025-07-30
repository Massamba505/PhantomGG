using PhantomGG.API.DTOs.RefreshToken;

namespace PhantomGG.API.DTOs.User;

public class UserWithTokensDto
{
    public UserProfileDto User { get; set; } = null!;
    public List<RefreshTokenDto> RefreshTokens { get; set; } = new();
}
