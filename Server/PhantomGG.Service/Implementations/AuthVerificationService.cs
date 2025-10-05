using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.Service.Implementations;

public class AuthVerificationService(
    IUserRepository userRepository,
    IEmailService emailService,
    IPasswordHasher passwordHasher) : IAuthVerificationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _userRepository.GetByEmailVerificationTokenAsync(token);
        if (user == null)
        {
            return false;
        }

        user.EmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiry = null;

        await _userRepository.UpdateAsync(user);
        await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName);

        return true;
    }

    public async Task<bool> ResendEmailVerificationAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLower());
        if (user == null || user.EmailVerified)
        {
            return false;
        }

        user.EmailVerificationToken = GenerateSecureToken();
        user.EmailVerificationTokenExpiry = DateTime.UtcNow.AddDays(1);

        await _userRepository.UpdateAsync(user);
        await _emailService.SendEmailVerificationAsync(user.Email, user.FirstName, user.EmailVerificationToken);

        return true;
    }

    public async Task<bool> InitiatePasswordResetAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email.ToLower());
        if (user == null || !user.IsActive)
        {
            return true;
        }

        user.PasswordResetToken = GenerateSecureToken();
        user.PasswordResetTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        await _userRepository.UpdateAsync(user);
        await _emailService.SendPasswordResetAsync(user.Email, user.FirstName, user.PasswordResetToken);

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        var user = await _userRepository.GetByPasswordResetTokenAsync(token);
        if (user == null)
        {
            return false;
        }

        if (!IsValidPassword(newPassword))
        {
            throw new ValidationException("Password must be at least 8 characters and contain uppercase, lowercase, number, and special character");
        }

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.FailedLoginAttempts = 0;
        user.AccountLockedUntil = null;

        await _userRepository.UpdateAsync(user);
        await _emailService.SendSecurityAlertAsync(user.Email, user.FirstName, "Your password has been successfully reset.");

        return true;
    }

    private static string GenerateSecureToken()
    {
        return Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(32));
    }

    private static bool IsValidPassword(string password)
    {
        return password.Length >= 8 &&
               password.Any(char.IsUpper) &&
               password.Any(char.IsLower) &&
               password.Any(char.IsDigit) &&
               password.Any(c => !char.IsLetterOrDigit(c));
    }
}