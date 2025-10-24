using PhantomGG.Service.Infrastructure.Email.Interfaces;

namespace PhantomGG.Service.Infrastructure.Email.Implementations;

public class EmailTemplateService : IEmailTemplateService
{
    private const string CommonStyles = @"
         body { font-family: Arial, sans-serif; background-color: #0f172a; padding: 20px; }
        .container { max-width: 600px; margin: auto; background: white; padding: 20px; border-radius: 10px; border: 1px solid #cde0d2; }
        .header { text-align: center; margin-bottom: 20px; }
        .header h1 { color: #0f172a; }
        .button { background-color: #3b82f6; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block; margin: 20px 0; }
        .footer { font-size: 12px; color: #666; margin-top: 30px; text-align: center; }
    ";

    public string GetEmailVerificationTemplate(string firstName, string verificationUrl)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>{CommonStyles}</style>
            </head>
            <body>
               <div class='container'>
                    <div class='header'>
                        <h1>⚽ PhantomGG</h1>
                    </div>
                    <h2>Time to Kick Off!</h2>
                    <p>Hi {firstName},</p>
                    <p>Thanks for joining PhantomGG, your ultimate soccer tournament platform!</p>
                    <p>Click below to verify your email and join the squad:</p>
                    <a href='{verificationUrl}' class='button'>Verify Email</a>
                    <p>This link will expire in 24 hours. If you didn't create an account, ignore this message.</p>
                    <div class='footer'>See you on the pitch!</div>
                </div>
            </body>
            </html>";
    }

    public string GetPasswordResetTemplate(string firstName, string resetUrl)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>{CommonStyles}</style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>⚽ PhantomGG</h1>
                    </div>
                    <h2>Regain Possession!</h2>
                    <p>Hi {firstName},</p>
                    <p>Forgot your password? No worries, every striker misses a shot.</p>
                    <p>Click below to reset your password:</p>
                    <a href='{resetUrl}' class='button'>Reset Password</a>
                    <p>This link will expire in 30 minutes. If you didn’t request this, ignore the email.</p>
                    <div class='footer'>Back in the game in no time!</div>
                </div>
            </body>
            </html>";
    }

    public string GetAccountLockedTemplate(string firstName, DateTime lockedUntil)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>{CommonStyles}</style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>⚽ PhantomGG</h1>
                    </div>
                    <h2>Red Card!</h2>
                    <p>Hi {firstName},</p>
                    <p>Your account has been temporarily locked due to multiple failed login attempts.</p>
                    <p>You can return to the match at: <strong>{lockedUntil:yyyy-MM-dd HH:mm} UTC</strong></p>
                    <p>If this wasn't you, contact support.</p>
                    <div class='footer'>Stay safe, champ!</div>
                </div>
            </body>
            </html>";
    }

    public string GetSecurityAlertTemplate(string firstName, string alertMessage)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>{CommonStyles}</style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>⚽ PhantomGG</h1>
                    </div>
                    <h2>Security Alert</h2>
                    <p>Hi {firstName},</p>
                    <p>{alertMessage}</p>
                    <p>If this wasn't you, secure your account immediately.</p>
                    <div class='footer'>Defense wins championships!</div>
                </div>
            </body>
            </html>";
    }

    public string GetWelcomeTemplate(string firstName, string host)
    {
        return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>{CommonStyles}</style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>⚽ PhantomGG</h1>
                    </div>
                    <h2>Welcome to the Team!</h2>
                    <p>Hi {firstName},</p>
                    <p>You’re now part of the PhantomGG squad!</p>
                    <p>Start organizing tournaments, building teams, and scoring goals!</p>
                    <a href='{host}' class='button'>Get Started</a>
                    <div class='footer'>Let's make history on the pitch ⚽</div>
                </div>
            </body>
            </html>";
    }
}
