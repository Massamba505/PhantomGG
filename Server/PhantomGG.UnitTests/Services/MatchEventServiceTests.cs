using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Service.Domain.Matches.Implementations;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Entities;
using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Common.Enums;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class MatchEventServiceTests
{
    private Mock<IMatchEventRepository> _matchEventRepositoryMock;
    private Mock<IMatchRepository> _matchRepositoryMock;
    private Mock<IMatchValidationService> _matchValidationServiceMock;
    private Mock<IMatchEventValidationService> _matchEventValidationServiceMock;
    private Mock<IPlayerValidationService> _playerValidationServiceMock;
    private Mock<ITeamValidationService> _teamValidationServiceMock;
    private Mock<ICacheInvalidationService> _cacheInvalidationServiceMock;
    private Mock<HybridCache> _cacheMock;
    private MatchEventService _matchEventService;

    [SetUp]
    public void Setup()
    {
        _matchEventRepositoryMock = new Mock<IMatchEventRepository>();
        _matchRepositoryMock = new Mock<IMatchRepository>();
        _matchValidationServiceMock = new Mock<IMatchValidationService>();
        _matchEventValidationServiceMock = new Mock<IMatchEventValidationService>();
        _playerValidationServiceMock = new Mock<IPlayerValidationService>();
        _teamValidationServiceMock = new Mock<ITeamValidationService>();
        _cacheInvalidationServiceMock = new Mock<ICacheInvalidationService>();
        _cacheMock = new Mock<HybridCache>();

        _matchEventService = new MatchEventService(
            _matchEventRepositoryMock.Object,
            _matchRepositoryMock.Object,
            _matchValidationServiceMock.Object,
            _matchEventValidationServiceMock.Object,
            _playerValidationServiceMock.Object,
            _teamValidationServiceMock.Object,
            _cacheInvalidationServiceMock.Object,
            _cacheMock.Object
        );
    }

    [Test]
    public async Task GetMatchEventsAsync_WhenEventsExist_ReturnsEvents()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        var matchEvents = new List<MatchEvent>
        {
            new MatchEvent
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                PlayerId = playerId,
                TeamId = teamId,
                EventType = (int)MatchEventType.Goal,
                Minute = 15
            },
            new MatchEvent
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                PlayerId = playerId,
                TeamId = teamId,
                EventType = (int)MatchEventType.YellowCard,
                Minute = 45
            }
        };

        var match = new Repository.Entities.Match
        {
            Id = matchId,
            TournamentId = Guid.NewGuid(),
            HomeTeamId = teamId,
            AwayTeamId = Guid.NewGuid(),
            MatchDate = DateTime.UtcNow,
            Status = (int)MatchStatus.InProgress
        };

        _matchValidationServiceMock
            .Setup(x => x.ValidateMatchExistsAsync(matchId))
            .ReturnsAsync(match);

        _matchEventRepositoryMock
            .Setup(x => x.GetByMatchIdAsync(matchId))
            .ReturnsAsync(matchEvents);

        // Act
        var result = await _matchEventService.GetMatchEventsAsync(matchId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.First().EventType.Should().Be(MatchEventType.Goal);
    }

    [Test]
    public async Task CreateMatchEventAsync_GoalEvent_RecalculatesScores()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var tournamentId = Guid.NewGuid();

        var createDto = new CreateMatchEventDto
        {
            MatchId = matchId,
            PlayerId = playerId,
            TeamId = teamId,
            EventType = MatchEventType.Goal,
            Minute = 20
        };

        var matchEvent = new MatchEvent
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            PlayerId = playerId,
            TeamId = teamId,
            EventType = (int)MatchEventType.Goal,
            Minute = 20
        };

        var match = new Repository.Entities.Match
        {
            Id = matchId,
            TournamentId = tournamentId,
            HomeTeamId = teamId,
            AwayTeamId = Guid.NewGuid(),
            HomeScore = 0,
            AwayScore = 0,
            MatchDate = DateTime.UtcNow,
            Status = (int)MatchStatus.InProgress
        };

        _matchValidationServiceMock
            .Setup(x => x.ValidateCanUpdateMatchAsync(matchId, userId))
            .ReturnsAsync(match);

        _matchValidationServiceMock
            .Setup(x => x.ValidatePlayerTeamRelationshipAsync(playerId, teamId, matchId))
            .Returns(Task.CompletedTask);

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateEventTimeAsync(createDto.Minute, matchId))
            .Returns(Task.CompletedTask);

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateEventTypeForMatchStatusAsync((int)createDto.EventType, matchId))
            .Returns(Task.CompletedTask);

        _matchEventRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<MatchEvent>()))
            .ReturnsAsync(matchEvent);

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(matchId))
            .ReturnsAsync(match);

        _matchEventRepositoryMock
            .Setup(x => x.GetByMatchIdAsync(matchId))
            .ReturnsAsync(new List<MatchEvent> { matchEvent });

        _matchRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Repository.Entities.Match>()))
            .ReturnsAsync(match);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidatePlayerStatsAsync(playerId, teamId, tournamentId))
            .Returns(Task.CompletedTask);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateMatchCacheAsync(matchId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _matchEventService.CreateMatchEventAsync(createDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.EventType.Should().Be(MatchEventType.Goal);
        _matchRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Repository.Entities.Match>()), Times.Once);
    }

    [Test]
    public async Task CreateMatchEventAsync_YellowCard_ValidatesYellowCardRules()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        var createDto = new CreateMatchEventDto
        {
            MatchId = matchId,
            PlayerId = playerId,
            TeamId = teamId,
            EventType = MatchEventType.YellowCard,
            Minute = 30
        };

        var matchEvent = new MatchEvent
        {
            Id = Guid.NewGuid(),
            MatchId = matchId,
            PlayerId = playerId,
            TeamId = teamId,
            EventType = (int)MatchEventType.YellowCard,
            Minute = 30
        };

        var match = new Repository.Entities.Match
        {
            Id = matchId,
            TournamentId = Guid.NewGuid(),
            HomeTeamId = teamId,
            AwayTeamId = Guid.NewGuid(),
            MatchDate = DateTime.UtcNow,
            Status = (int)MatchStatus.InProgress
        };

        _matchValidationServiceMock
            .Setup(x => x.ValidateCanUpdateMatchAsync(matchId, userId))
            .ReturnsAsync(match);

        _matchValidationServiceMock
            .Setup(x => x.ValidatePlayerTeamRelationshipAsync(playerId, teamId, matchId))
            .Returns(Task.CompletedTask);

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateEventTimeAsync(createDto.Minute, matchId))
            .Returns(Task.CompletedTask);

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateEventTypeForMatchStatusAsync((int)createDto.EventType, matchId))
            .Returns(Task.CompletedTask);

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateYellowCardRulesAsync(playerId, matchId))
            .Returns(Task.CompletedTask);

        _matchEventRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<MatchEvent>()))
            .ReturnsAsync(matchEvent);

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(matchId))
            .ReturnsAsync((Repository.Entities.Match?)null);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateMatchCacheAsync(matchId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _matchEventService.CreateMatchEventAsync(createDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.EventType.Should().Be(MatchEventType.YellowCard);
        _matchEventValidationServiceMock.Verify(x => x.ValidateYellowCardRulesAsync(playerId, matchId), Times.Once);
    }

    [Test]
    public async Task DeleteMatchEventAsync_GoalEvent_RecalculatesScores()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var tournamentId = Guid.NewGuid();

        var matchEvent = new MatchEvent
        {
            Id = eventId,
            MatchId = matchId,
            PlayerId = playerId,
            TeamId = teamId,
            EventType = (int)MatchEventType.Goal,
            Minute = 20
        };

        var match = new Repository.Entities.Match
        {
            Id = matchId,
            TournamentId = tournamentId,
            HomeTeamId = teamId,
            AwayTeamId = Guid.NewGuid(),
            HomeScore = 1,
            AwayScore = 0,
            MatchDate = DateTime.UtcNow,
            Status = (int)MatchStatus.InProgress
        };

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateMatchEventExistsAsync(eventId))
            .ReturnsAsync(matchEvent);

        _matchValidationServiceMock
            .Setup(x => x.ValidateCanUpdateMatchAsync(matchId, userId))
            .ReturnsAsync(match);

        _matchEventRepositoryMock
            .Setup(x => x.DeleteAsync(eventId))
            .Returns(Task.CompletedTask);

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(matchId))
            .ReturnsAsync(match);

        _matchEventRepositoryMock
            .Setup(x => x.GetByMatchIdAsync(matchId))
            .ReturnsAsync(new List<MatchEvent>());

        _matchRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Repository.Entities.Match>()))
            .ReturnsAsync(match);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidatePlayerStatsAsync(playerId, teamId, tournamentId))
            .Returns(Task.CompletedTask);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateMatchCacheAsync(matchId))
            .Returns(Task.CompletedTask);

        // Act
        await _matchEventService.DeleteMatchEventAsync(eventId, userId);

        // Assert
        _matchEventRepositoryMock.Verify(x => x.DeleteAsync(eventId), Times.Once);
        _matchRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Repository.Entities.Match>()), Times.Once);
    }

    [Test]
    public async Task UpdateMatchEventAsync_ChangeEventType_UpdatesSuccessfully()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var matchId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();

        var existingEvent = new MatchEvent
        {
            Id = eventId,
            MatchId = matchId,
            PlayerId = playerId,
            TeamId = teamId,
            EventType = (int)MatchEventType.YellowCard,
            Minute = 30
        };

        var updateDto = new UpdateMatchEventDto
        {
            EventType = MatchEventType.RedCard
        };

        var match = new Repository.Entities.Match
        {
            Id = matchId,
            TournamentId = Guid.NewGuid(),
            HomeTeamId = teamId,
            AwayTeamId = Guid.NewGuid(),
            MatchDate = DateTime.UtcNow,
            Status = (int)MatchStatus.InProgress
        };

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateMatchEventExistsAsync(eventId))
            .ReturnsAsync(existingEvent);

        _matchValidationServiceMock
            .Setup(x => x.ValidateCanUpdateMatchAsync(matchId, userId))
            .ReturnsAsync(match);

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateEventTypeForMatchStatusAsync((int)updateDto.EventType.Value, matchId))
            .Returns(Task.CompletedTask);

        _matchEventValidationServiceMock
            .Setup(x => x.ValidateRedCardRulesAsync(playerId, matchId))
            .Returns(Task.CompletedTask);

        var updatedEvent = new MatchEvent
        {
            Id = eventId,
            MatchId = matchId,
            PlayerId = playerId,
            TeamId = teamId,
            EventType = (int)MatchEventType.RedCard,
            Minute = 30
        };

        _matchEventRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<MatchEvent>()))
            .ReturnsAsync(updatedEvent);

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(matchId))
            .ReturnsAsync((Repository.Entities.Match)null);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateMatchCacheAsync(matchId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _matchEventService.UpdateMatchEventAsync(eventId, updateDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.EventType.Should().Be(MatchEventType.RedCard);
        _matchEventValidationServiceMock.Verify(x => x.ValidateRedCardRulesAsync(playerId, matchId), Times.Once);
    }
}
