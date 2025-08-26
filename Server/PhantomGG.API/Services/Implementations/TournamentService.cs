using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;
using PhantomGG.API.Mappings;

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
        return tournaments.Select(tournament => tournament.ToTournamentDto());
    }

    public async Task<TournamentDto> GetByIdAsync(Guid id)
    {
        var tournament = await _tournamentRepository.GetByIdAsync(id);
        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        return tournament.ToTournamentDto();
    }

    public async Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        return tournaments.Select(tournament => tournament.ToTournamentDto());
    }

    public async Task<IEnumerable<TournamentDto>> SearchAsync(TournamentSearchDto searchDto)
    {
        var tournaments = await _tournamentRepository.SearchAsync(searchDto);
        return tournaments.Select(tournament => tournament.ToTournamentDto());
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        var tournament = createDto.ToTournament(organizerId);
        var createdTournament = await _tournamentRepository.CreateAsync(tournament);
        return createdTournament.ToTournamentDto();
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

        existingTournament.UpdateFromDto(updateDto);
        var updatedTournament = await _tournamentRepository.UpdateAsync(existingTournament);
        return updatedTournament.ToTournamentDto();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        if (!await _tournamentRepository.ExistsAsync(id))
            throw new NotFoundException("Tournament not found");

        if (!await _tournamentRepository.IsOrganizerAsync(id, userId))
            throw new ForbiddenException("You are not authorized to delete this tournament");

        await _tournamentRepository.DeleteAsync(id);
    }
}
