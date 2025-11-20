using PhantomGG.Repository.Entities;

namespace PhantomGG.Repository.Interfaces;

public interface ITournamentTeamRepository
{
    Task<IEnumerable<TournamentTeam>> GetByTournamentAsync(Guid tournamentId);
    Task<IEnumerable<TournamentTeam>> GetByTournamentAndStatusAsync(Guid tournamentId, int status);
    Task<IEnumerable<TournamentTeam>> GetByTeamIdsAsync(List<Guid> teamIds);
    Task<TournamentTeam?> GetRegistrationAsync(Guid tournamentId, Guid teamId);
    Task<bool> IsTeamRegisteredAsync(Guid tournamentId, Guid teamId);
    Task<TournamentTeam> CreateAsync(TournamentTeam tournamentTeam);
    Task<TournamentTeam> UpdateAsync(TournamentTeam tournamentTeam);
    Task DeleteAsync(TournamentTeam tournamentTeam);
    Task<int> GetCountByStatusAsync(Guid tournamentId, int status);
    Task<IEnumerable<Tournament>> GetTournamentsByTeamAsync(Guid teamId);
}
