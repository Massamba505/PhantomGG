using Microsoft.AspNetCore.Http;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.TournamentFormat;

namespace PhantomGG.Service.Interfaces;

public interface ITournamentService
{
    Task<IEnumerable<TournamentDto>> GetAllAsync();
    Task<IEnumerable<TournamentDto>> GetAllPublicAsync();
    Task<IEnumerable<TournamentFormatDto>> GetAllFormatsAsync();
    Task<TournamentDto> GetByIdAsync(Guid id);
    Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId);
    Task<IEnumerable<TournamentDto>> SearchAsync(TournamentSearchDto searchDto);
    Task<PaginatedResponse<TournamentDto>> SearchWithPaginationAsync(TournamentSearchDto searchDto, Guid? userId = null);
    Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId);
    Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid userId);
    Task DeleteAsync(Guid id, Guid userId);
    Task<TournamentDto> StartTournamentAsync(Guid id, Guid userId);
    Task<TournamentDto> CompleteTournamentAsync(Guid id, Guid userId);
    Task<TournamentDto> CancelTournamentAsync(Guid id, Guid userId);
    Task<TournamentDto> OpenRegistrationAsync(Guid id, Guid userId);
    Task<TournamentDto> CloseRegistrationAsync(Guid id, Guid userId);
    Task<bool> CanTeamRegisterAsync(Guid tournamentId, Guid teamId);
    Task<bool> IsRegistrationOpenAsync(Guid tournamentId);
    Task<int> GetRegisteredTeamCountAsync(Guid tournamentId);
    Task<int> GetApprovedTeamCountAsync(Guid tournamentId);
    Task JoinTournamentAsync(Guid tournamentId, Guid teamId, Guid userId);
    Task LeaveTournamentAsync(Guid tournamentId, Guid teamId, Guid userId);
    Task<string> UploadTournamentBannerAsync(Guid tournamentId, IFormFile file, Guid userId);
    Task<string> UploadTournamentLogoAsync(Guid tournamentId, IFormFile file, Guid userId);
    Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId);
    Task<bool> CanUserManageTournamentAsync(Guid tournamentId, Guid userId);
}
