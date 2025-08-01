using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class PasswordService : IPasswordService
{
    public PasswordHashResult CreatePasswordHash(string password)
    {
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return new PasswordHashResult(hashedPassword, salt);
    }

    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedHash);
    }
}