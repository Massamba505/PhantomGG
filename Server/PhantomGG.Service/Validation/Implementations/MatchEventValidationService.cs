using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Validation.Implementations;

public class MatchEventValidationService(
    IMatchEventRepository matchEventRepository,
    IMatchRepository matchRepository,
    IPlayerRepository playerRepository) : IMatchEventValidationService
{
    private readonly IMatchEventRepository _matchEventRepository = matchEventRepository;
    private readonly IMatchRepository _matchRepository = matchRepository;
    private readonly IPlayerRepository _playerRepository = playerRepository;

    public async Task<MatchEvent> ValidateMatchEventExistsAsync(Guid eventId)
    {
        var matchEvent = await _matchEventRepository.GetByIdAsync(eventId);
        if (matchEvent == null)
            throw new NotFoundException("Match event not found");

        return matchEvent;
    }

    public async Task<MatchEvent> ValidateCanUpdateEventAsync(Guid eventId, Guid userId)
    {
        var matchEvent = await ValidateMatchEventExistsAsync(eventId);
        var match = await _matchRepository.GetByIdAsync(matchEvent.MatchId);

        if (match == null)
            throw new NotFoundException("Match not found");

        if (match.Tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to update this match event");

        if (match.Status == (int)MatchStatus.Completed)
            throw new ForbiddenException("Cannot update events for completed matches");

        return matchEvent;
    }

    public async Task<MatchEvent> ValidateCanDeleteEventAsync(Guid eventId, Guid userId)
    {
        var matchEvent = await ValidateMatchEventExistsAsync(eventId);
        var match = await _matchRepository.GetByIdAsync(matchEvent.MatchId);

        if (match == null)
            throw new NotFoundException("Match not found");

        if (match.Tournament.OrganizerId != userId)
            throw new ForbiddenException("You don't have permission to delete this match event");

        if (match.Status == (int)MatchStatus.Completed)
            throw new ForbiddenException("Cannot delete events from completed matches");

        return matchEvent;
    }

    public async Task ValidateEventTimeAsync(int minute, Guid matchId)
    {
        if (minute < 0)
            throw new ValidationException("Event minute cannot be negative");

        if (minute > 120)
            throw new ValidationException("Event minute cannot exceed 120 minutes");

        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        if (match.Status == (int)MatchStatus.Scheduled)
            throw new ValidationException("Cannot add events to scheduled matches");
    }

    public async Task ValidatePlayerInMatchAsync(Guid playerId, Guid matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException("Player not found");

        if (player.TeamId != match.HomeTeamId && player.TeamId != match.AwayTeamId)
            throw new ValidationException("Player's team is not participating in this match");
    }

    public async Task ValidatePlayerTeamInMatchAsync(Guid playerId, Guid teamId, Guid matchId)
    {
        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException("Player not found");

        if (player.TeamId != teamId)
            throw new ValidationException("Player does not belong to the specified team");

        await ValidatePlayerInMatchAsync(playerId, matchId);
    }

    public async Task ValidateRedCardRulesAsync(Guid playerId, Guid matchId)
    {
        var events = await _matchEventRepository.GetByPlayerIdAsync(playerId);
        var matchEvents = events.Where(e => e.MatchId == matchId);

        var hasRedCard = matchEvents.Any(e => e.EventType == (int)MatchEventType.RedCard);
        if (hasRedCard)
            throw new ValidationException("Player already has a red card in this match");

        var yellowCards = matchEvents.Count(e => e.EventType == (int)MatchEventType.YellowCard);
        if (yellowCards >= 2)
            throw new ValidationException("Player already has two yellow cards. Cannot add another red card");
    }

    public async Task ValidateYellowCardRulesAsync(Guid playerId, Guid matchId)
    {
        var events = await _matchEventRepository.GetByPlayerIdAsync(playerId);
        var matchEvents = events.Where(e => e.MatchId == matchId);

        var hasRedCard = matchEvents.Any(e => e.EventType == (int)MatchEventType.RedCard);
        if (hasRedCard)
            throw new ValidationException("Player has been sent off. Cannot add more cards");

        var yellowCards = matchEvents.Count(e => e.EventType == (int)MatchEventType.YellowCard);
        if (yellowCards >= 2)
            throw new ValidationException("Player already has two yellow cards. Add a red card instead");
    }

    public async Task ValidateGoalScorerAsync(Guid playerId, int eventType)
    {
        if (eventType != (int)MatchEventType.Goal)
            return;

        var player = await _playerRepository.GetByIdAsync(playerId);
        if (player == null)
            throw new NotFoundException("Player not found");

        if (player.Position == (int)PlayerPosition.Goalkeeper)
        {
            await Task.CompletedTask;
        }
    }

    public async Task ValidateEventTypeForMatchStatusAsync(int eventType, Guid matchId)
    {
        var match = await _matchRepository.GetByIdAsync(matchId);
        if (match == null)
            throw new NotFoundException("Match not found");

        if (match.Status == (int)MatchStatus.Scheduled)
            throw new ValidationException("Cannot add events to a match that hasn't started");

        if (match.Status == (int)MatchStatus.Cancelled)
            throw new ValidationException("Cannot add events to a cancelled match");

        if (match.Status == (int)MatchStatus.Postponed)
            throw new ValidationException("Cannot add events to a postponed match");
    }
}
