namespace PhantomGG.Service.Interfaces;

public interface IEmailTemplateService
{
    string GetEmailVerificationTemplate(string firstName, string verificationUrl);
    string GetPasswordResetTemplate(string firstName, string resetUrl);
    string GetAccountLockedTemplate(string firstName, DateTime lockedUntil);
    string GetSecurityAlertTemplate(string firstName, string alertMessage);
    string GetWelcomeTemplate(string firstName, string host);
}
