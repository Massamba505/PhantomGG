using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Validation.Implementations;

public class UserValidationService(
    IUserRepository userRepository,
    ITournamentRepository tournamentRepository,
    ITeamRepository teamRepository) : IUserValidationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITeamRepository _teamRepository = teamRepository;

    public async Task<User> ValidateUserExistsAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");

        return user;
    }

    public async Task<User> ValidateUserIsActiveAsync(Guid userId)
    {
        var user = await ValidateUserExistsAsync(userId);

        if (!user.IsActive)
            throw new ForbiddenException("User account is not active. Please contact support");

        return user;
    }

    public async Task<User> ValidateEmailVerifiedAsync(Guid userId)
    {
        var user = await ValidateUserExistsAsync(userId);

        if (!user.EmailVerified)
            throw new ForbiddenException("Email must be verified before performing this action");

        return user;
    }

    public async Task ValidateAccountNotLockedAsync(Guid userId)
    {
        var user = await ValidateUserExistsAsync(userId);

        if (user.AccountLockedUntil.HasValue && user.AccountLockedUntil.Value > DateTime.UtcNow)
        {
            var minutesRemaining = (int)(user.AccountLockedUntil.Value - DateTime.UtcNow).TotalMinutes;
            throw new ForbiddenException($"Account is locked. Please try again in {minutesRemaining} minutes");
        }
    }

    public async Task ValidateMaxTournamentsPerUserAsync(Guid userId, int maxTournaments = 10)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(userId);

        if (tournaments.Count() >= maxTournaments)
            throw new ForbiddenException($"You have reached the maximum limit of {maxTournaments} tournaments. Please delete or complete some tournaments before creating new ones");
    }

    public async Task ValidateMaxTeamsPerUserAsync(Guid userId, int maxTeams = 5)
    {
        var teams = await _teamRepository.GetByUserAsync(userId);

        if (teams.Count() >= maxTeams)
            throw new ForbiddenException($"You have reached the maximum limit of {maxTeams} teams. Please delete some teams before creating new ones");
    }

    public async Task ValidateUserCanManageResourceAsync(Guid userId, Guid resourceOwnerId, string resourceType)
    {
        if (userId != resourceOwnerId)
            throw new ForbiddenException($"You don't have permission to manage this {resourceType}");

        await Task.CompletedTask;
    }

    public async Task ValidatePasswordResetTokenAsync(string token, Guid userId)
    {
        var user = await ValidateUserExistsAsync(userId);

        if (string.IsNullOrEmpty(user.PasswordResetToken))
            throw new ValidationException("No password reset token found");

        if (user.PasswordResetToken != token)
            throw new ValidationException("Invalid password reset token");

        if (!user.PasswordResetTokenExpiry.HasValue || user.PasswordResetTokenExpiry.Value < DateTime.UtcNow)
            throw new ValidationException("Password reset token has expired. Please request a new one");
    }

    public async Task ValidateEmailVerificationTokenAsync(string token, Guid userId)
    {
        var user = await ValidateUserExistsAsync(userId);

        if (user.EmailVerified)
            throw new ValidationException("Email is already verified");

        if (string.IsNullOrEmpty(user.EmailVerificationToken))
            throw new ValidationException("No email verification token found");

        if (user.EmailVerificationToken != token)
            throw new ValidationException("Invalid email verification token");

        if (!user.EmailVerificationTokenExpiry.HasValue || user.EmailVerificationTokenExpiry.Value < DateTime.UtcNow)
            throw new ValidationException("Email verification token has expired. Please request a new one");
    }
}
