using PhantomGG.API.DTOs.TournamentFormat;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class TournamentFormatService(ITournamentFormatRepository tournamentFormatRepository) : ITournamentFormatService
{
    private ITournamentFormatRepository _tournamentFormatRepository = tournamentFormatRepository;

    public async Task<IEnumerable<TournamentFormatDto>> GetAllActiveAsync()
    {
        var formats = await _tournamentFormatRepository.GetAllAsync();
        return formats.Select(f => new TournamentFormatDto {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
        }) ;
    }
}
