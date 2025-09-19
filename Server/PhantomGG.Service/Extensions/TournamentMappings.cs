using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.Entities;

namespace PhantomGG.Service.Mappings;

public static class TournamentMappings
{
    public static TournamentDto ToDto(this Tournament tournament)
    {
        return new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Description = tournament.Description,
            Location = tournament.Location,
            RegistrationStartDate = tournament.RegistrationStartDate,
            RegistrationDeadline = tournament.RegistrationDeadline,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            MinTeams = tournament.MinTeams,
            MaxTeams = tournament.MaxTeams,
            BannerUrl = tournament.BannerUrl,
            LogoUrl = tournament.LogoUrl,
            Status = tournament.Status,
            OrganizerId = tournament.OrganizerId,
            Organizer = tournament.Organizer?.ToOrganizerDto(),
            CreatedAt = tournament.CreatedAt,
            UpdatedAt = tournament.UpdatedAt,
            IsPublic = tournament.IsPublic,
            TeamCount = tournament.TournamentTeams?.Count ?? 0,
            MatchCount = tournament.Matches?.Count ?? 0
        };
    }

    public static Tournament ToEntity(this CreateTournamentDto createDto, Guid organizerId)
    {
        return new Tournament
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            Description = createDto.Description,
            Location = createDto.Location,
            RegistrationStartDate = createDto.RegistrationStartDate,
            RegistrationDeadline = createDto.RegistrationDeadline,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            MinTeams = createDto.MinTeams,
            MaxTeams = createDto.MaxTeams,
            BannerUrl = createDto.BannerUrl ?? $"https://placehold.co/1200x400?text={createDto.Name}",
            LogoUrl = createDto.LogoUrl ?? "https://placehold.co/200x200",
            Status = TournamentStatus.Draft.ToString(),
            OrganizerId = organizerId,
            IsPublic = createDto.IsPublic,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(this UpdateTournamentDto updateDto, Tournament tournament)
    {
        if (!string.IsNullOrEmpty(updateDto.Name))
            tournament.Name = updateDto.Name;
        if (!string.IsNullOrEmpty(updateDto.Description))
            tournament.Description = updateDto.Description;
        if (updateDto.Location != null)
            tournament.Location = updateDto.Location;
        if (updateDto.RegistrationDeadline.HasValue)
            tournament.RegistrationDeadline = updateDto.RegistrationDeadline;
        if (updateDto.StartDate.HasValue)
            tournament.StartDate = updateDto.StartDate.Value;
        if (updateDto.MaxTeams.HasValue)
            tournament.MaxTeams = updateDto.MaxTeams.Value;
        if (updateDto.BannerUrl != null)
            tournament.BannerUrl = updateDto.BannerUrl;
        if (updateDto.LogoUrl != null)
            tournament.LogoUrl = updateDto.LogoUrl;
        if (updateDto.IsPublic.HasValue)
            tournament.IsPublic = updateDto.IsPublic.Value;

        tournament.UpdatedAt = DateTime.UtcNow;
    }
}