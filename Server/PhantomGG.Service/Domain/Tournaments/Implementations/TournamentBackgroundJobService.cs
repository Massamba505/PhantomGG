using Microsoft.Extensions.Logging;
using PhantomGG.Service.Domain.Tournaments.Interfaces;

namespace PhantomGG.Service.Domain.Tournaments.Implementations;

public class TournamentBackgroundJobService(
    ITournamentService tournamentService,
    ILogger<TournamentBackgroundJobService> logger) : ITournamentBackgroundJobService
{
    private readonly ITournamentService _tournamentService = tournamentService;
    private readonly ILogger<TournamentBackgroundJobService> _logger = logger;

    public async Task UpdateTournamentStatusesAsync()
    {
        _logger.LogInformation("Starting background job: UpdateTournamentStatusesAsync at {Timestamp}", DateTime.UtcNow);

        try
        {
            await _tournamentService.UpdateTournamentStatusesAsync();
            _logger.LogInformation("Successfully completed background job: UpdateTournamentStatusesAsync");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during background job: UpdateTournamentStatusesAsync");
            throw;
        }
    }
}
