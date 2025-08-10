using PhantomGG.API.DTOs.Auth;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations
{
    public class PasswordService : IPasswordService
    {
        public PasswordHashResult CreatePasswordHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("Password cannot be null or empty.");
            }

            string salt = BCrypt.Net.BCrypt.GenerateSalt();
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            return new PasswordHashResult(hashedPassword, salt);
        }

        public bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(storedHash) || string.IsNullOrWhiteSpace(storedSalt))
            {
                return false;
            }

            try
            {
                string hashToCompare = BCrypt.Net.BCrypt.HashPassword(password, storedSalt);

                return hashToCompare == storedHash;
            }
            catch
            {
                return false;
            }
        }
    }
}
