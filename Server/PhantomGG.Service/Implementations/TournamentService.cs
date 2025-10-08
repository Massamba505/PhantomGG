using Microsoft.Extensions.Caching.Hybrid;
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
    HybridCache cache) : ITournamentService
{
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly ITournamentValidationService _validationService = validationService;
    private readonly IImageService _imageService = imageService;
    private readonly HybridCache _cache = cache;

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
            IsPublic = userId.HasValue ? null : true,
            Page = query.Page,
            PageSize = query.PageSize
        };

        string cacheKey = spec.GetDeterministicKey();

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                var result = await _tournamentRepository.SearchAsync(spec);
                return new PagedResult<TournamentDto>(
                    result.Data.Select(t => t.ToDto()),
                    result.Meta.Page,
                    result.Meta.PageSize,
                    result.Meta.TotalRecords
                );
            },
            new HybridCacheEntryOptions { Expiration = TimeSpan.FromMinutes(5) }
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

        return updatedTournament.ToDto();
    }

    public async Task DeleteAsync(Guid id, Guid organizerId)
    {
        await _validationService.ValidateCanDeleteAsync(id, organizerId);
        await _tournamentRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<TournamentDto>> GetByOrganizerAsync(Guid organizerId)
    {
        var tournaments = await _tournamentRepository.GetByOrganizerAsync(organizerId);
        return tournaments.Select(t => t.ToDto());
    }
}
