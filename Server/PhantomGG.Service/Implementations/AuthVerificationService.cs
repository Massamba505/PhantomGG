using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.Service.Implementations;

public class AuthVerificationService(
    IUserRepository userRepository,
    IEmailService emailService,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : IAuthVerificationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly IPasswordHasher _passwordHasher = passwordHasher;
    private readonly ITokenService _tokenService = tokenService;

    public async Task<bool> VerifyEmailAsync(string token)
    {
        var user = await _userRepository.GetByEmailVerificationTokenAsync(token);
        if (user == null)
        {
            return false;
        }

        if (user.EmailVerified)
        {
            throw new UnauthorizedException("Email is already verified.");
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
        if (user == null)
        {
            return false;
        }

        if(user.EmailVerified)
        {
            throw new UnauthorizedException("Email is already verified.");
        }

        var emailVerificationToken = _tokenService.GenerateEmailVerificationToken();
        user.EmailVerificationToken = emailVerificationToken.Token;
        user.EmailVerificationTokenExpiry = emailVerificationToken.ExpiresAt;

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

        var passwordResetToken = _tokenService.GeneratePasswordResetToken();
        user.PasswordResetToken = passwordResetToken.Token;
        user.PasswordResetTokenExpiry = passwordResetToken.ExpiresAt;

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

        user.PasswordHash = _passwordHasher.HashPassword(newPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiry = null;
        user.FailedLoginAttempts = 0;
        user.AccountLockedUntil = null;

        await _userRepository.UpdateAsync(user);
        await _emailService.SendSecurityAlertAsync(user.Email, user.FirstName, "Your password has been successfully reset.");

        return true;
    }
}