using PhantomGG.API.DTOs.Auth;

namespace PhantomGG.API.Services.Interfaces;

public interface IPasswordService
{
    PasswordHashResult CreatePasswordHash(string password);
    bool VerifyPassword(string password, string storedHash, string storedSalt);
}