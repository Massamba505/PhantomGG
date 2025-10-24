using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using PhantomGG.Common.Config;
using PhantomGG.Service.Infrastructure.Email.Interfaces;

namespace PhantomGG.Service.Infrastructure.Email.Implementations;

public class EmailService(
    IOptions<EmailSettings> emailSettings,
    IEmailTemplateService templateService) : IEmailService
{
    private readonly EmailSettings _emailSettings = emailSettings.Value;
    private readonly IEmailTemplateService _templateService = templateService;

    public async Task SendEmailVerificationAsync(string email, string firstName, string verificationToken)
    {
        var baseUrl = GetHost();
        var verificationUrl = $"{baseUrl}/auth/verify-email?token={verificationToken}";
        var htmlContent = _templateService.GetEmailVerificationTemplate(firstName, verificationUrl);
        await SendEmailAsync(email, "Verify Your Email Address", htmlContent);
    }

    public async Task SendPasswordResetAsync(string email, string firstName, string resetToken)
    {
        var baseUrl = GetHost();
        var resetUrl = $"{baseUrl}/auth/reset-password?token={resetToken}";
        var htmlContent = _templateService.GetPasswordResetTemplate(firstName, resetUrl);
        await SendEmailAsync(email, "Reset Your Password", htmlContent);
    }

    public async Task SendAccountLockedAsync(string email, string firstName, DateTime lockedUntil)
    {
        var htmlContent = _templateService.GetAccountLockedTemplate(firstName, lockedUntil);
        await SendEmailAsync(email, "Account Security Alert", htmlContent);
    }

    public async Task SendSecurityAlertAsync(string email, string firstName, string alertMessage)
    {
        var htmlContent = _templateService.GetSecurityAlertTemplate(firstName, alertMessage);
        await SendEmailAsync(email, "Security Alert", htmlContent);
    }

    public async Task SendWelcomeEmailAsync(string email, string firstName)
    {
        var baseUrl = GetHost();
        var htmlContent = _templateService.GetWelcomeTemplate(firstName, baseUrl);
        await SendEmailAsync(email, "Welcome to PhantomGG", htmlContent);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlContent
        };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort,
            _emailSettings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    private string GetHost()
    {
        return _emailSettings.FrontendBaseUrl ?? "http://localhost:4200";
    }
}
