using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Models.Entities;

namespace PhantomGG.API.Mappings;

public static class MatchEventMappings
{
    public static MatchEventDto ToMatchEventDto(this MatchEventDto matchEvent)
    {
        return new MatchEventDto
        {
            Id = matchEvent.Id,
            MatchId = matchEvent.MatchId,
            EventType = matchEvent.EventType,
            Minute = matchEvent.Minute,
            TeamId = matchEvent.TeamId,
            //TeamName = matchEvent.Team?.Name ?? string.Empty,
            PlayerName = matchEvent.PlayerName,
            Description = matchEvent.Description
        };
    }

    public static MatchEventDto ToMatchEvent(this CreateMatchEventDto createDto)
    {
        return new MatchEventDto
        {
            Id = Guid.NewGuid(),
            MatchId = createDto.MatchId,
            EventType = createDto.EventType,
            Minute = createDto.Minute,
            TeamId = createDto.TeamId,
            PlayerName = createDto.PlayerName,
            Description = createDto.Description
        };
    }

    public static void UpdateFromDto(this MatchEvent matchEvent, UpdateMatchEventDto updateDto)
    {
        matchEvent.Minute = updateDto.Minute;
        matchEvent.PlayerName = updateDto.PlayerName;
        matchEvent.Description = updateDto.Description;
    }
}
