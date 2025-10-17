using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs;
using PhantomGG.Models.DTOs.Image;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Specifications;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Interfaces;
using PhantomGG.Service.Mappings;

namespace PhantomGG.Service.Implementations;

public class TournamentService(
    ITournamentRepository tournamentRepository,
    IImageService imageService,
    ITournamentValidationService validationService,
    ICacheInvalidationService cacheInvalidationService,
    HybridCache cache,
    ILogger<TournamentService> logger) : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITournamentValidationService _validationService = validationService;
    private readonly IImageService _imageService = imageService;
    private readonly HybridCache _cache = cache;
    private readonly ICacheInvalidationService _cacheInvalidationService = cacheInvalidationService;
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
            IsPublic = !userId.HasValue || query.IsPublic,
            Page = query.Page,
            PageSize = query.PageSize
        };

        var result = await _tournamentRepository.SearchAsync(spec);
        return new PagedResult<TournamentDto>(
            result.Data.Select(t => t.ToDto()),
            result.Meta.Page,
            result.Meta.PageSize,
            result.Meta.TotalRecords
        );
    }

    public async Task<TournamentDto> GetByIdAsync(Guid id)
    {
        string cacheKey = $"tournament_{id}";
        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                var tournament = await _tournamentRepository.GetByIdAsync(id);
                if (tournament == null)
                    throw new NotFoundException($"Tournament not found");

                return tournament.ToDto();
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(10) }
        );
    }

    public async Task<TournamentDto> CreateAsync(CreateTournamentDto createDto, Guid organizerId)
    {
        await ValidateMaxTournamentsPerUserAsync(organizerId);

        var tournament = createDto.ToEntity(organizerId);
        if (createDto.BannerUrl != null)
        {
            var uploadImage = new UploadImageRequest
            {
                OldFileUrl = tournament.BannerUrl,
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
                OldFileUrl = tournament.BannerUrl,
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
        var activeTournaments = tournaments.Where(t => t.Status != (int)TournamentStatus.Completed).ToList();

        if (activeTournaments.Count >= 5)
        {
            throw new ForbiddenException("You cannot create more than 5 active tournaments. Please complete or cancel an existing tournament first.");
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

    private static TournamentStatus DetermineNewStatus(PhantomGG.Repository.Entities.Tournament tournament, DateTime now)
    {
        var currentStatus = (TournamentStatus)tournament.Status;

        if (currentStatus == TournamentStatus.Draft && now >= tournament.RegistrationStartDate)
        {
            return TournamentStatus.RegistrationOpen;
        }

        if (currentStatus == TournamentStatus.RegistrationOpen && now >= tournament.RegistrationDeadline)
        {
            return TournamentStatus.RegistrationClosed;
        }

        if (currentStatus == TournamentStatus.RegistrationClosed && now >= tournament.StartDate)
        {
            return TournamentStatus.InProgress;
        }

        if (currentStatus == TournamentStatus.InProgress &&
             tournament.EndDate.HasValue &&
             now >= tournament.EndDate.Value)
        {
            return TournamentStatus.Completed;
        }

        return currentStatus;
    }
}
