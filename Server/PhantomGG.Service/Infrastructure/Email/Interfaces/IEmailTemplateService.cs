namespace PhantomGG.Service.Infrastructure.Email.Interfaces;

public interface IEmailTemplateService
{
    string GetEmailVerificationTemplate(string firstName, string verificationUrl);
    string GetPasswordResetTemplate(string firstName, string resetUrl);
    string GetAccountLockedTemplate(string firstName, DateTime lockedUntil);
    string GetSecurityAlertTemplate(string firstName, string alertMessage);
    string GetWelcomeTemplate(string firstName, string host);
    string GetTeamRegistrationRequestTemplate(string organizerName, string teamName, string tournamentName);
    string GetTeamApprovedTemplate(string teamManagerName, string teamName, string tournamentName, DateTime? startDate);
    string GetTeamRejectedTemplate(string teamManagerName, string teamName, string tournamentName);
    string GetTournamentStatusChangedTemplate(string organizerName, string tournamentName, string oldStatus, string newStatus);
}
