using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Models;

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
            RegistrationDeadline = tournament.RegistrationDeadline,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            MaxTeams = tournament.MaxTeams,
            EntryFee = tournament.EntryFee,
            Prize = tournament.Prize,
            ContactEmail = tournament.ContactEmail,
            BannerUrl = tournament.BannerUrl,
            Status = tournament.Status,
            Organizer = tournament.Organizer,
            OrganizerName = tournament.OrganizerNavigation?.FirstName + " " + tournament.OrganizerNavigation?.LastName ?? "Unknown",
            TeamCount = tournament.Teams?.Count ?? 0,
            CreatedAt = tournament.CreatedAt
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
            RegistrationDeadline = dto.RegistrationDeadline,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            MaxTeams = dto.MaxTeams,
            EntryFee = dto.EntryFee,
            Prize = dto.Prize,
            ContactEmail = dto.ContactEmail,
            BannerUrl = dto.BannerUrl,
            Status = "Active",
            Organizer = organizerId,
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
        tournament.EndDate = dto.EndDate;
        tournament.MaxTeams = dto.MaxTeams;
        tournament.EntryFee = dto.EntryFee;
        tournament.Prize = dto.Prize;
        tournament.ContactEmail = dto.ContactEmail;
        tournament.BannerUrl = dto.BannerUrl;
        if (!string.IsNullOrEmpty(dto.Status))
            tournament.Status = dto.Status;
    }
}
