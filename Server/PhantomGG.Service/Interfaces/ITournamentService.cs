using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.TournamentStanding;
using Microsoft.AspNetCore.Http;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentService
{
    #region Public Tournament Operations (from Controller)
    Task<PaginatedResponse<TournamentDto>> SearchAsync(TournamentSearchDto searchDto);
    Task<TournamentDto> GetByIdAsync(Guid id);
    Task<IEnumerable<TeamDto>> GetTournamentTeamsAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> GetTournamentMatchesAsync(Guid tournamentId);
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId);
    #endregion

    #region User Operations (from Controller)
    Task RegisterForTournamentAsync(Guid tournamentId, object registrationDto, Guid userId);
    Task WithdrawFromTournamentAsync(Guid tournamentId, object withdrawDto, Guid userId);
    #endregion

    #region Organizer Operations (from Controller)
    Task<PaginatedResponse<TournamentDto>> GetMyTournamentsAsync(TournamentSearchDto searchDto, Guid organizerId);
    Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId);
    Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid organizerId);
    Task DeleteAsync(Guid id, Guid organizerId);
    Task<string> UploadImageAsync(Guid tournamentId, IFormFile file, Guid organizerId);

    // Team Management
    Task<IEnumerable<TeamDto>> GetPendingTeamsAsync(Guid tournamentId, Guid organizerId);
    Task ApproveTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId);
    Task RejectTeamAsync(Guid tournamentId, Guid teamId, PhantomGG.Models.DTOs.Team.RejectTeamDto rejectDto, Guid organizerId);
    Task RemoveTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId);

    // Match Management
    Task CreateTournamentBracketAsync(Guid tournamentId, Guid organizerId);
    Task UpdateMatchResultAsync(Guid tournamentId, Guid matchId, object resultDto, Guid organizerId);
    #endregion
}
