using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.Service.Implementations;

public class TournamentStandingService(
    ITournamentStandingRepository tournamentStandingRepository,
    ITournamentValidationService validationService) : ITournamentStandingService
{
    private readonly ITournamentStandingRepository _tournamentStandingRepository = tournamentStandingRepository;
    private readonly ITournamentValidationService _validationService = validationService;

    public async Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);
        return await _tournamentStandingRepository.GetByTournamentAsync(tournamentId);
    }

    public async Task<IEnumerable<PlayerGoalStandingDto>> GetPlayerGoalStandingsAsync(Guid tournamentId)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);
        return await _tournamentStandingRepository.GetPlayerGoalStandingsAsync(tournamentId);
    }

    public async Task<IEnumerable<PlayerAssistStandingDto>> GetPlayerAssistStandingsAsync(Guid tournamentId)
    {
        await _validationService.ValidateTournamentExistsAsync(tournamentId);
        return await _tournamentStandingRepository.GetPlayerAssistStandingsAsync(tournamentId);
    }
}
