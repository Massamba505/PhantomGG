using PhantomGG.API.Models;
using PhantomGG.API.DTOs.TournamentFormat;

namespace PhantomGG.API.Mappings;

public static class TournamentFormatMappings
{
    public static TournamentFormatDto ToTournamentFormatDto(this TournamentFormat format)
    {
        return new TournamentFormatDto
        {
            Id = format.Id,
            Name = format.Name,
            Description = format.Description,
            MinTeams = format.MinTeams,
            MaxTeams = format.MaxTeams,
            IsActive = format.IsActive
        };
    }
}
