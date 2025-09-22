using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.TournamentStanding;
using Microsoft.AspNetCore.Http;
using PhantomGG.Models.Entities;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentService
{
    Task<PaginatedResponse<TournamentDto>> SearchAsync(TournamentSearchDto searchDto);
    Task<TournamentDto> GetByIdAsync(Guid id);
    Task<IEnumerable<TournamentTeamDto>> GetTournamentTeamsAsync(Guid tournamentId);
    Task<IEnumerable<MatchDto>> GetTournamentMatchesAsync(Guid tournamentId);
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId);
    Task RegisterForTournamentAsync(Guid tournamentId, JoinTournamentDto registrationDto, Guid userId);
    Task WithdrawFromTournamentAsync(Guid tournamentId, LeaveTournamentDto withdrawDto, Guid userId);

    #region Organizer Operations (from Controller)
    Task<PaginatedResponse<TournamentDto>> GetMyTournamentsAsync(TournamentSearchDto searchDto, Guid organizerId);
    Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId);
    Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid organizerId);
    Task DeleteAsync(Guid id, Guid organizerId);
    Task<string> UploadImageAsync(Tournament tournamentId, IFormFile file);
    Task<string> UploadLogoImageAsync(Tournament tournamentId, IFormFile file);

    // Team Management
    Task<IEnumerable<TournamentTeamDto>> GetPendingTeamsAsync(Guid tournamentId, Guid organizerId);
    Task ApproveTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId);
    Task RejectTeamAsync(Guid tournamentId, Guid teamId, Guid organizerId);

    // Match Management
    Task CreateTournamentBracketAsync(Guid tournamentId, Guid organizerId);
    Task UpdateMatchResultAsync(Guid tournamentId, Guid matchId, object resultDto, Guid organizerId);
    #endregion
}
