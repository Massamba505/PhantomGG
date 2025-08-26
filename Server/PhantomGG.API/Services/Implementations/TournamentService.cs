using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class TournamentService : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository;

    public TournamentService(ITournamentRepository tournamentRepository)
    {
        _tournamentRepository = tournamentRepository;
    }

    public async Task<IEnumerable<TournamentDto>> GetAllAsync()
    {
        var tournaments = await _tournamentRepository.GetAllAsync();
        return tournaments.Select(MapToDto);
    }

    public async Task<TournamentDto> GetByIdAsync(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        return MapToDto(tournament);
    }

    public async Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        return tournaments.Select(MapToDto);
    }

    public async Task<IEnumerable<TournamentDto>> SearchAsync(TournamentSearchDto searchDto)
    {
        var tournaments = await _tournamentRepository.SearchAsync(searchDto);
        return tournaments.Select(MapToDto);
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        var tournament = MapFromCreateDto(createDto, organizerId);
        var createdTournament = await _tournamentRepository.CreateAsync(tournament);
        return MapToDto(createdTournament);
    }

    public async Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid userId)
    {
        if (!await _tournamentRepository.ExistsAsync(id))
            throw new NotFoundException("Tournament not found");

        if (!await _tournamentRepository.IsOrganizerAsync(id, userId))
            throw new ForbiddenException("You are not authorized to modify this tournament");

        var existingTournament = await _tournamentRepository.GetByIdAsync(id);
        if (existingTournament == null)
            throw new NotFoundException("Tournament not found");

        UpdateTournamentFromDto(existingTournament, updateDto);
        var updatedTournament = await _tournamentRepository.UpdateAsync(existingTournament);
        return MapToDto(updatedTournament);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        if (!await _tournamentRepository.ExistsAsync(id))
            throw new NotFoundException("Tournament not found");

        if (!await _tournamentRepository.IsOrganizerAsync(id, userId))
            throw new ForbiddenException("You are not authorized to delete this tournament");

        await _tournamentRepository.DeleteAsync(id);
    }

    private static TournamentDto MapToDto(Tournament tournament)
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

    private static Tournament MapFromCreateDto(CreateTournamentDto dto, Guid organizerId)
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

    private static void UpdateTournamentFromDto(Tournament tournament, UpdateTournamentDto dto)
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
