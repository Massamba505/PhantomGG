namespace PhantomGG.Service.Interfaces;

public interface ICacheInvalidationService
{
    Task InvalidateUserCacheAsync(Guid userId);
    Task InvalidateTournamentCacheAsync(Guid tournamentId);
    Task InvalidateTeamCacheAsync(Guid teamId);
    Task InvalidateTournamentRelatedCacheAsync(Guid tournamentId);
    Task InvalidateTeamRelatedCacheAsync(Guid teamId);
}