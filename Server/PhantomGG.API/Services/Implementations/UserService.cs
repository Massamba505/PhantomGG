using PhantomGG.API.DTOs.User;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Mappings;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using BC = BCrypt.Net.BCrypt;

namespace PhantomGG.API.Services.Implementations;

public class UserService(IUserRepository userRepository, ITournamentRepository tournamentRepository) : IUserService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;

    public async Task<UserDto> GetByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        return user.ToUserDto();
    }

    public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        // Check if email is being changed and if it already exists
        if (user.Email != request.Email)
        {
            var emailExists = await _userRepository.EmailExistsAsync(request.Email);
            if (emailExists)
            {
                throw new ConflictException("Email address is already in use");
            }
        }

        // Update user properties
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;

        await _userRepository.UpdateAsync(user);
        return user.ToUserDto();
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        // Verify current password
        if (!BC.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw new ValidationException("Current password is incorrect");
        }

        // Hash new password
        user.PasswordHash = BC.HashPassword(request.NewPassword);
        await _userRepository.UpdateAsync(user);
    }

    public async Task<UserStatsDto> GetUserStatsAsync(Guid userId)
    {
        // Get tournaments created by this user (as organizer)
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(userId);

        var activeTournaments = tournaments.Count(t => t.Status == "active" || t.Status == "draft");
        var completedTournaments = tournaments.Count(t => t.Status == "completed");

        // For teams managed, we need to count teams across all tournaments
        // This would typically be done with a proper join or separate repository method
        var teamsManaged = 0; // Placeholder - would need team repository to get accurate count

        return new UserStatsDto
        {
            ActiveTournaments = activeTournaments,
            TeamsManaged = teamsManaged,
            CompletedTournaments = completedTournaments
        };
    }

    public async Task<UserActivityDto> GetUserActivityAsync(Guid userId, int page, int limit)
    {
        // This would typically come from an activity log table
        // For now, we'll generate some mock data based on user's tournaments
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(userId);

        var activities = tournaments
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(t => new ActivityItemDto
            {
                Id = Guid.NewGuid().ToString(),
                Type = "tournament",
                Message = $"Created tournament \"{t.Name}\"",
                Date = t.CreatedAt.ToString("MMMM dd, yyyy"),
                EntityId = t.Id.ToString()
            })
            .ToList();

        return new UserActivityDto
        {
            Activities = activities,
            TotalCount = tournaments.Count()
        };
    }

    public async Task<ProfilePictureUploadDto> UploadProfilePictureAsync(Guid userId, IFormFile file)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException("User not found");
        }

        // Generate a unique filename
        var fileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var uploadPath = Path.Combine("wwwroot", "uploads", "profile-pictures");

        // Ensure directory exists
        Directory.CreateDirectory(uploadPath);

        var filePath = Path.Combine(uploadPath, fileName);

        // Save the file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Update user's profile picture URL
        var profilePictureUrl = $"/uploads/profile-pictures/{fileName}";
        user.ProfilePictureUrl = profilePictureUrl;
        await _userRepository.UpdateAsync(user);

        return new ProfilePictureUploadDto
        {
            ProfilePictureUrl = profilePictureUrl
        };
    }
}
