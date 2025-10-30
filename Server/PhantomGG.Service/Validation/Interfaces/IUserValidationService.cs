using PhantomGG.Repository.Entities;

namespace PhantomGG.Service.Validation.Interfaces;

public interface IUserValidationService
{
    Task<User> ValidateUserExistsAsync(Guid userId);
    Task<User> ValidateUserIsActiveAsync(Guid userId);
    Task<User> ValidateEmailVerifiedAsync(Guid userId);
    Task ValidateAccountNotLockedAsync(Guid userId);
    Task ValidateMaxTournamentsPerUserAsync(Guid userId, int maxTournaments = 10);
    Task ValidateMaxTeamsPerUserAsync(Guid userId, int maxTeams = 5);
    Task ValidateUserCanManageResourceAsync(Guid userId, Guid resourceOwnerId, string resourceType);
    Task ValidatePasswordResetTokenAsync(string token, Guid userId);
    Task ValidateEmailVerificationTokenAsync(string token, Guid userId);
}
