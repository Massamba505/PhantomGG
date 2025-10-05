namespace PhantomGG.Service.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string firstName, string verificationToken);
    Task SendPasswordResetAsync(string email, string firstName, string resetToken);
    Task SendAccountLockedAsync(string email, string firstName, DateTime lockedUntil);
    Task SendSecurityAlertAsync(string email, string firstName, string alertMessage);
    Task SendWelcomeEmailAsync(string email, string firstName);
}
