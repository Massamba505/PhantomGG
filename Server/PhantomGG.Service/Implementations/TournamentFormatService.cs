using PhantomGG.Service.Interfaces;
using PhantomGG.Models.DTOs.TournamentFormat;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Service.Implementations;

public class TournamentFormatService(ITournamentFormatRepository tournamentFormatRepository) : ITournamentFormatService
{
    private ITournamentFormatRepository _tournamentFormatRepository = tournamentFormatRepository;

    public async Task<IEnumerable<TournamentFormatDto>> GetAllActiveAsync()
    {
        var formats = await _tournamentFormatRepository.GetAllAsync();
        return formats.Select(f => new TournamentFormatDto
        {
            Id = f.Id,
            Name = f.Name,
            Description = f.Description,
        });
    }
}
