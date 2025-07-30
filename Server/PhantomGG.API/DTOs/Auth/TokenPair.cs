namespace PhantomGG.API.DTOs.Auth;

public class TokenPair
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}