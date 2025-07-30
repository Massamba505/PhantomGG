using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;
using System.Security.Cryptography;

namespace PhantomGG.API.Services.Implementations;

public class PasswordService : IPasswordService
{

    public PasswordHashResult CreatePasswordHash(string password)
    {
        byte[] salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256);

        byte[] hash = pbkdf2.GetBytes(32);

        return new PasswordHashResult(Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool VerifyPassword(string password, string storedHash, string storedSalt)
    {
        byte[] salt = Convert.FromBase64String(storedSalt);
        byte[] hashBytes = Convert.FromBase64String(storedHash);

        var pbkdf2 = new Rfc2898DeriveBytes(
            password,
            salt,
            100000,
            HashAlgorithmName.SHA256);

        byte[] computedHash = pbkdf2.GetBytes(32);

        return CryptographicOperations.FixedTimeEquals(computedHash, hashBytes);
    }
}