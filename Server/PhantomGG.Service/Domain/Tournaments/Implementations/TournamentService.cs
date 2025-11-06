using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Image;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Infrastructure.Email.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Mappings;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Domain.Tournaments.Interfaces;

namespace PhantomGG.Service.Domain.Tournaments.Implementations;

public class TournamentService(
    ITournamentRepository tournamentRepository,
    IImageService imageService,
    ITournamentValidationService validationService,
    ICacheInvalidationService cacheInvalidationService,
    IUserRepository userRepository,
    IEmailService emailService,
    HybridCache cache,
    ILogger<TournamentService> logger) : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITournamentValidationService _validationService = validationService;
    private readonly IImageService _imageService = imageService;
    private readonly HybridCache _cache = cache;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<TournamentService> _logger = logger;

    public async Task<PagedResult<TournamentDto>> SearchAsync(TournamentQuery query, Guid? userId = null)
    {
        var spec = new TournamentSpecification
        {
            SearchTerm = query.Q,
            Status = query.Status,
            Location = query.Location,
            StartDateFrom = query.StartFrom,
            StartDateTo = query.StartTo,
            OrganizerId = userId,
            IsPublic = query.IsPublic,
            Page = query.Page,
            PageSize = query.PageSize
        };

        var result = await _tournamentRepository.SearchAsync(spec);

        var filteredData = result.Data;
        if (!userId.HasValue)
        {
            filteredData = filteredData.Where(t => t.Status != (int)TournamentStatus.Draft).ToList();
        }

        return new PagedResult<TournamentDto>(
            filteredData.Select(t => t.ToDto()),
            result.Meta.Page,
            result.Meta.PageSize,
            filteredData.Count()
        );
    }

    public async Task<TournamentDto> GetByIdAsync(Guid id, Guid? currentUserId = null)
    {
        string cacheKey = $"tournament_{id}";
        var tournament = await _tournamentRepository.GetByIdAsync(id);

        if (tournament == null)
            throw new NotFoundException("Tournament not found");

        if (tournament.Status == (int)TournamentStatus.Draft)
        {
            if (!currentUserId.HasValue || tournament.OrganizerId != currentUserId.Value)
                throw new ForbiddenException("Draft tournaments are only visible to their organizers");
        }

        return tournament.ToDto();
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        await _validationService.ValidateTournamentDatesAsync(createDto.StartDate, createDto.EndDate);

        await ValidateMaxTournamentsPerUserAsync(organizerId);

        var tournament = createDto.ToEntity(organizerId);
        if (createDto.BannerUrl != null)
        {
            var uploadImage = new UploadImageRequest
            {
                OldFileUrl = null,
                File = createDto.BannerUrl,
                ImageType = ImageType.TournamentBanner,
                Id = tournament.Id
            };

            tournament.BannerUrl = await _imageService.UploadImageAsync(uploadImage);
        }

        if (createDto.LogoUrl != null)
        {
            var uploadImage = new UploadImageRequest
            {
                OldFileUrl = null,
                File = createDto.LogoUrl,
                ImageType = ImageType.TournamentBanner,
                Id = tournament.Id
            };

            tournament.LogoUrl = await _imageService.UploadImageAsync(uploadImage);
        }

        var createdTournament = await _tournamentRepository.CreateAsync(tournament);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournament.Id);

        return createdTournament.ToDto();
    }

    public async Task<TournamentDto> UpdateAsync(Guid id, UpdateTournamentDto updateDto, Guid organizerId)
    {
        var tournament = await _validationService.ValidateCanUpdateAsync(id, organizerId);

        updateDto.UpdateEntity(tournament);

        if (updateDto.BannerUrl != null)
        {
            var uploadImage = new UploadImageRequest
            {
                OldFileUrl = tournament.BannerUrl,
                File = updateDto.BannerUrl,
                ImageType = ImageType.TournamentBanner,
                Id = tournament.Id
            };

            tournament.BannerUrl = await _imageService.UploadImageAsync(uploadImage);
        }

        if (updateDto.LogoUrl != null)
        {
            var uploadImage = new UploadImageRequest
            {
                OldFileUrl = tournament.BannerUrl,
                File = updateDto.LogoUrl,
                ImageType = ImageType.TournamentBanner,
                Id = tournament.Id
            };

            tournament.LogoUrl = await _imageService.UploadImageAsync(uploadImage);
        }

        var updatedTournament = await _tournamentRepository.UpdateAsync(tournament);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournament.Id);

        return updatedTournament.ToDto();
    }

    public async Task DeleteAsync(Guid id, Guid organizerId)
    {
        var tournament = await _validationService.ValidateCanDeleteAsync(id, organizerId);
        await _tournamentRepository.DeleteAsync(id);
        await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournament.Id);
    }

    public async Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        return tournaments.Select(t => t.ToDto());
    }

    private async Task ValidateMaxTournamentsPerUserAsync(Guid organizerId)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        var activeTournaments = tournaments.Where(t =>
            t.Status != (int)TournamentStatus.Completed &&
            t.Status != (int)TournamentStatus.Cancelled).ToList();

        const int maxActiveTournaments = 10;
        if (activeTournaments.Count >= maxActiveTournaments)
        {
            throw new ForbiddenException($"You cannot create more than {maxActiveTournaments} active tournaments. Please complete or cancel an existing tournament first.");
        }
    }

    public async Task UpdateTournamentStatusesAsync()
    {
        _logger.LogInformation("Starting tournament status update job at {Timestamp}", DateTime.UtcNow);

        try
        {
            var now = DateTime.UtcNow;
            var updatedCount = 0;

            var spec = new TournamentSpecification
            {
                Status = null,
                Page = 1,
                PageSize = int.MaxValue
            };

            var allTournaments = await _tournamentRepository.SearchAsync(spec);
            var tournamentsToUpdate = allTournaments.Data.Where(t =>
                t.Status != (int)TournamentStatus.Completed &&
                t.Status != (int)TournamentStatus.Cancelled).ToList();

            foreach (var tournament in tournamentsToUpdate)
            {
                var oldStatus = (TournamentStatus)tournament.Status;
                var newStatus = DetermineNewStatus(tournament, now);

                if (oldStatus != newStatus)
                {
                    _logger.LogInformation("Updating tournament {TournamentId} ({TournamentName}) status from {OldStatus} to {NewStatus}",
                        tournament.Id, tournament.Name, oldStatus, newStatus);

                    tournament.Status = (int)newStatus;
                    tournament.UpdatedAt = now;

                    await _tournamentRepository.UpdateAsync(tournament);
                    await _cacheInvalidationService.InvalidateTournamentRelatedCacheAsync(tournament.Id);

                    try
                    {
                        var organizer = await _userRepository.GetByIdAsync(tournament.OrganizerId);
                        if (organizer != null)
                        {
                            await _emailService.SendTournamentStatusChangedAsync(
                                organizer.Email,
                                organizer.FirstName,
                                tournament.Name,
                                oldStatus.ToString(),
                                newStatus.ToString());
                        }
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogWarning(emailEx, "Failed to send status change email for tournament {TournamentId}: {EmailError}",
                            tournament.Id, emailEx.Message);
                    }

                    updatedCount++;
                }
            }

            _logger.LogInformation("Tournament status update job completed. Updated {UpdatedCount} tournaments", updatedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during tournament status update job");
            throw;
        }
    }

    private static TournamentStatus DetermineNewStatus(Tournament tournament, DateTime now)
    {
        if (now < tournament.RegistrationStartDate)
            return TournamentStatus.Draft;

        if (now < tournament.RegistrationDeadline)
            return TournamentStatus.RegistrationOpen;

        if (now < tournament.StartDate)
            return TournamentStatus.RegistrationClosed;

        if (tournament.EndDate.HasValue && now < tournament.EndDate.Value)
            return TournamentStatus.InProgress;

        return TournamentStatus.Completed;
    }
}
