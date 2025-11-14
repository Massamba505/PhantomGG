using Moq;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Domain.Matches.Implementations;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class MatchServiceTests
{
    private Mock<IMatchRepository> _mockMatchRepository = null!;
    private Mock<IMatchEventRepository> _mockMatchEventRepository = null!;
    private Mock<ITournamentTeamRepository> _mockTournamentTeamRepository = null!;
    private Mock<IMatchValidationService> _mockMatchValidationService = null!;
    private Mock<ITournamentValidationService> _mockTournamentValidationService = null!;
    private Mock<ICacheInvalidationService> _mockCacheInvalidationService = null!;
    private Mock<HybridCache> _mockCache = null!;
    private MatchService _matchService = null!;

    [SetUp]
    public void Setup()
    {
        _mockMatchRepository = new Mock<IMatchRepository>();
        _mockMatchEventRepository = new Mock<IMatchEventRepository>();
        _mockTournamentTeamRepository = new Mock<ITournamentTeamRepository>();
        _mockMatchValidationService = new Mock<IMatchValidationService>();
        _mockTournamentValidationService = new Mock<ITournamentValidationService>();
        _mockCacheInvalidationService = new Mock<ICacheInvalidationService>();
        _mockCache = new Mock<HybridCache>();

        _matchService = new MatchService(
            _mockMatchRepository.Object,
            _mockMatchEventRepository.Object,
            _mockTournamentTeamRepository.Object,
            _mockMatchValidationService.Object,
            _mockTournamentValidationService.Object,
            _mockCacheInvalidationService.Object,
            _mockCache.Object
        );
    }

    [Test]
    public async Task GIVEN_ValidMatchId_WHEN_GetByIdAsync_THEN_ReturnsMatchDto()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var tournamentId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();

        var match = new Repository.Entities.Match
        {
            Id = matchId,
            TournamentId = tournamentId,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            MatchDate = DateTime.UtcNow.AddDays(7),
            Status = (int)MatchStatus.Scheduled,
            Tournament = new Tournament { Id = tournamentId, Name = "Test Tournament" },
            HomeTeam = new Team { Id = homeTeamId, Name = "Home Team" },
            AwayTeam = new Team { Id = awayTeamId, Name = "Away Team" }
        };

        _mockMatchValidationService
            .Setup(x => x.ValidateMatchExistsAsync(matchId))
            .ReturnsAsync(match);

        // Act
        var result = await _matchService.GetByIdAsync(matchId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(matchId));
        Assert.That(result.Status, Is.EqualTo(MatchStatus.Scheduled));
    }

    [Test]
    public async Task GIVEN_ValidCreateDto_WHEN_CreateAsync_THEN_CreatesMatchSuccessfully()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var createDto = new CreateMatchDto
        {
            TournamentId = tournamentId,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            MatchDate = DateTime.UtcNow.AddDays(7)
        };

        var tournament = new Tournament { Id = tournamentId, Name = "Test Tournament" };

        _mockMatchValidationService
            .Setup(x => x.ValidateTournamentForMatchAsync(tournamentId, userId))
            .ReturnsAsync(tournament);

        _mockMatchValidationService
            .Setup(x => x.ValidateTeamsCanPlayAsync(homeTeamId, awayTeamId, tournamentId))
            .Returns(Task.CompletedTask);

        _mockMatchValidationService
            .Setup(x => x.ValidateMatchSchedulingAsync(homeTeamId, awayTeamId, createDto.MatchDate, It.IsAny<Guid?>()))
            .Returns(Task.CompletedTask);

        _mockMatchRepository
            .Setup(x => x.CreateAsync(It.IsAny<Repository.Entities.Match>()))
            .ReturnsAsync((Repository.Entities.Match m) => m);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTournamentRelatedCacheAsync(tournamentId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _matchService.CreateAsync(createDto, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TournamentId, Is.EqualTo(tournamentId));
        _mockMatchRepository.Verify(x => x.CreateAsync(It.IsAny<Repository.Entities.Match>()), Times.Once);
    }

    [Test]
    public async Task GIVEN_ValidMatchResult_WHEN_UpdateResultAsync_THEN_UpdatesMatchSuccessfully()
    {
        // Arrange
        var matchId = Guid.NewGuid();
        var tournamentId = Guid.NewGuid();
        var homeTeamId = Guid.NewGuid();
        var awayTeamId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var match = new Repository.Entities.Match
        {
            Id = matchId,
            TournamentId = tournamentId,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            MatchDate = DateTime.UtcNow.AddDays(-1),
            Status = (int)MatchStatus.InProgress
        };

        var resultDto = new MatchResultDto
        {
            Status = MatchStatus.Completed
        };

        var matchEvents = new List<MatchEvent>
        {
            new MatchEvent
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                TeamId = homeTeamId,
                EventType = (int)MatchEventType.Goal
            },
            new MatchEvent
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                TeamId = homeTeamId,
                EventType = (int)MatchEventType.Goal
            },
            new MatchEvent
            {
                Id = Guid.NewGuid(),
                MatchId = matchId,
                TeamId = awayTeamId,
                EventType = (int)MatchEventType.Goal
            }
        };

        _mockMatchValidationService
            .Setup(x => x.ValidateCanUpdateResultAsync(matchId, userId))
            .ReturnsAsync(match);

        _mockMatchEventRepository
            .Setup(x => x.GetByMatchIdAsync(matchId))
            .ReturnsAsync(matchEvents);

        _mockMatchRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Repository.Entities.Match>()))
            .ReturnsAsync((Repository.Entities.Match m) => m);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateMatchCacheAsync(matchId))
            .Returns(Task.CompletedTask);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTournamentRelatedCacheAsync(tournamentId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _matchService.UpdateResultAsync(matchId, resultDto, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Status, Is.EqualTo(MatchStatus.Completed));
        _mockMatchRepository.Verify(x => x.UpdateAsync(It.IsAny<Repository.Entities.Match>()), Times.Once);
    }
}
