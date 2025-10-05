namespace PhantomGG.Service.Interfaces;

public interface IAuthVerificationService
{
    Task<bool> VerifyEmailAsync(string token);
    Task<bool> ResendEmailVerificationAsync(string email);
    Task<bool> InitiatePasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(string token, string newPassword);
}
