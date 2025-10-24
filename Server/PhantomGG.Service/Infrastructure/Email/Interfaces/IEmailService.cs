namespace PhantomGG.Service.Infrastructure.Email.Interfaces;

public interface IEmailService
{
    Task SendEmailVerificationAsync(string email, string firstName, string verificationToken);
    Task SendPasswordResetAsync(string email, string firstName, string resetToken);
    Task SendAccountLockedAsync(string email, string firstName, DateTime lockedUntil);
    Task SendSecurityAlertAsync(string email, string firstName, string alertMessage);
    Task SendWelcomeEmailAsync(string email, string firstName);
    Task SendTeamRegistrationRequestAsync(string organizerEmail, string organizerName, string teamName, string tournamentName);
    Task SendTeamApprovedAsync(string teamManagerEmail, string teamManagerName, string teamName, string tournamentName);
    Task SendTeamRejectedAsync(string teamManagerEmail, string teamManagerName, string teamName, string tournamentName);
    Task SendTournamentStatusChangedAsync(string organizerEmail, string organizerName, string tournamentName, string oldStatus, string newStatus);
}
