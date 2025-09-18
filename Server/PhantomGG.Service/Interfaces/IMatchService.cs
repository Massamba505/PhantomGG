using PhantomGG.Models.DTOs.Match;

namespace PhantomGG.Service.Interfaces;

public interface IMatchService
{
    #region Tournament Match Operations (from Controller)
    Task<IEnumerable<MatchDto>> GetByTournamentAsync(Guid tournamentId);
    Task<MatchDto> UpdateResultAsync(Guid matchId, object resultDto, Guid organizerId);
    Task<IEnumerable<MatchDto>> GenerateTournamentBracketAsync(Guid tournamentId, Guid organizerId);
    #endregion

    #region Match CRUD Operations
    Task<MatchDto> CreateAsync(CreateMatchDto createDto, Guid userId);
    Task<MatchDto> UpdateAsync(Guid id, UpdateMatchDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    #endregion

    #region Fixture Generation Operations
    Task<IEnumerable<MatchDto>> GenerateRoundRobinFixturesAsync(GenerateFixturesDto generateDto, Guid userId);
    Task<IEnumerable<MatchDto>> AutoGenerateFixturesAsync(AutoGenerateFixturesDto generateDto, Guid userId);
    Task<FixtureGenerationStatusDto> GetFixtureGenerationStatusAsync(Guid tournamentId);
    #endregion
}
