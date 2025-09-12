using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.Entities;

namespace PhantomGG.API.Mappings;

public static class TournamentMappings
{
    public static TournamentDto ToTournamentDto(this Tournament tournament)
    {
        return new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Description = tournament.Description,
            Location = tournament.Location,
            FormatId = tournament.FormatId,
            FormatName = tournament.Format?.Name ?? "Unknown",
            RegistrationStartDate = tournament.RegistrationStartDate,
            RegistrationDeadline = tournament.RegistrationDeadline,
            StartDate = tournament.StartDate,
            MinTeams = tournament.MinTeams,
            MaxTeams = tournament.MaxTeams,
            MaxPlayersPerTeam = tournament.MaxPlayersPerTeam,
            MinPlayersPerTeam = tournament.MinPlayersPerTeam,
            EntryFee = tournament.EntryFee,
            PrizePool = tournament.PrizePool,
            ContactEmail = tournament.ContactEmail,
            BannerUrl = tournament.BannerUrl,
            LogoUrl = tournament.LogoUrl,
            Status = tournament.Status,
            MatchDuration = tournament.MatchDuration ?? 90,
            OrganizerId = tournament.OrganizerId,
            OrganizerName = tournament.Organizer?.FirstName + " " + tournament.Organizer?.LastName ?? "Unknown",
            CreatedAt = tournament.CreatedAt,
            UpdatedAt = tournament.UpdatedAt,
            IsActive = tournament.IsActive,
            IsPublic = tournament.IsPublic,
            TeamCount = tournament.Teams?.Count ?? 0,
            MatchCount = tournament.Matches?.Count ?? 0,
            CompletedMatches = tournament.Matches?.Count(m => m.Status == "Completed") ?? 0
        };
    }

    public static Tournament ToTournament(this CreateTournamentDto dto, Guid organizerId)
    {
        return new Tournament
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            Location = dto.Location,
            FormatId = dto.FormatId,
            RegistrationStartDate = dto.RegistrationStartDate,
            RegistrationDeadline = dto.RegistrationDeadline,
            StartDate = dto.StartDate,
            MinTeams = dto.MinTeams,
            MaxTeams = dto.MaxTeams,
            MaxPlayersPerTeam = dto.MaxPlayersPerTeam,
            MinPlayersPerTeam = dto.MinPlayersPerTeam,
            EntryFee = dto.EntryFee,
            PrizePool = dto.PrizePool,
            ContactEmail = dto.ContactEmail,
            BannerUrl = dto.BannerUrl,
            LogoUrl = dto.LogoUrl,
            MatchDuration = dto.MatchDuration,
            IsPublic = dto.IsPublic,
            Status = "Draft",
            OrganizerId = organizerId,
            IsActive = true
        };
    }

    public static void UpdateFromDto(this Tournament tournament, UpdateTournamentDto dto)
    {
        tournament.Name = dto.Name;
        tournament.Description = dto.Description;
        tournament.Location = dto.Location;
        tournament.RegistrationDeadline = dto.RegistrationDeadline;
        tournament.StartDate = dto.StartDate;
        tournament.MaxTeams = dto.MaxTeams;
        tournament.EntryFee = dto.EntryFee;
        tournament.PrizePool = dto.PrizePool;
        tournament.ContactEmail = dto.ContactEmail;
        tournament.BannerUrl = dto.BannerUrl;
        tournament.LogoUrl = dto.LogoUrl;
        tournament.MatchDuration = dto.MatchDuration;
        tournament.IsPublic = dto.IsPublic;
        if (!string.IsNullOrEmpty(dto.Status))
            tournament.Status = dto.Status;
        tournament.UpdatedAt = DateTime.UtcNow;
    }
}
