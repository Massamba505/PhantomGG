using Microsoft.AspNetCore.Http;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.TournamentStanding;
using PhantomGG.Models.Entities;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentService
{
    Task<PaginatedResponse<TournamentDto>> SearchAsync(TournamentSearchDto searchDto, Guid? userId = null);
    Task<TournamentDto> GetByIdAsync(Guid id);
    Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId);
    Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid organizerId);
    Task DeleteAsync(Guid id, Guid organizerId);
    Task<IEnumerable<TournamentTeamDto>> GetTournamentTeamsAsync(Guid tournamentId, Guid? userId = null, TeamRegistrationStatus status = TeamRegistrationStatus.Approved);
    Task UpdateTournamentStatusAsync(Guid tournamentId, Guid userId, TournamentStatus status);
    Task<IEnumerable<MatchDto>> GetTournamentMatchesAsync(Guid tournamentId, Guid? userId = null);
    Task<IEnumerable<TournamentStandingDto>> GetTournamentStandingsAsync(Guid tournamentId);
    Task ManageTeamAsync(Guid tournamentId, Guid? teamId, TeamActionDto actionDto, Guid userId);
    Task CreateTournamentBracketAsync(Guid tournamentId, Guid organizerId);
    Task<string> UploadImageAsync(Tournament tournament, IFormFile file);
    Task<string> UploadLogoImageAsync(Tournament tournament, IFormFile file);
}
