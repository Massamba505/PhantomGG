using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Repository.Entities;
using PhantomGG.Common.Enums;

namespace PhantomGG.Service.Mappings;

public static class MatchEventMappings
{
    public static MatchEventDto ToDto(this MatchEvent matchEvent)
    {
        return new MatchEventDto
        {
            Id = matchEvent.Id,
            MatchId = matchEvent.MatchId,
            EventType = matchEvent.EventType,
            Minute = matchEvent.Minute,
            TeamId = matchEvent.TeamId,
            TeamName = matchEvent.Team?.Name ?? "Unknown",
            PlayerId = matchEvent.PlayerId,
            PlayerName = $"{matchEvent.Player?.FirstName} {matchEvent.Player?.LastName}".Trim()
        };
    }

    public static MatchEvent ToEntity(this CreateMatchEventDto createDto)
    {
        return new MatchEvent
        {
            MatchId = createDto.MatchId,
            EventType = createDto.EventType.ToString(),
            Minute = createDto.Minute,
            TeamId = createDto.TeamId,
            PlayerId = createDto.PlayerId
        };
    }

    public static void UpdateEntity(this UpdateMatchEventDto updateDto, MatchEvent matchEvent)
    {
        if (updateDto.EventType.HasValue)
            matchEvent.EventType = updateDto.EventType.Value.ToString();

        if (updateDto.Minute.HasValue)
            matchEvent.Minute = updateDto.Minute.Value;

        if (updateDto.PlayerId.HasValue)
            matchEvent.PlayerId = updateDto.PlayerId.Value;
    }
}