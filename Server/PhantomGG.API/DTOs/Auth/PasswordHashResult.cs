namespace PhantomGG.API.DTOs.Auth;

public class PasswordHashResult
{
    public string Hash { get; }
    public string Salt { get; }
    public PasswordHashResult(string hash, string salt)
    {
        Hash = hash;
        Salt = salt;
    }
}
