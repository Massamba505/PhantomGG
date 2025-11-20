using Microsoft.Extensions.Caching.Hybrid;
using Moq;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Domain.Teams.Implementations;
using PhantomGG.Service.Domain.Teams.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class TeamServiceTests
{
    private Mock<ITeamRepository> _mockTeamRepository = null!;
    private Mock<ITournamentTeamRepository> _mockTournamentTeamRepository = null!;
    private Mock<IImageService> _mockImageService = null!;
    private Mock<IPlayerService> _mockPlayerService = null!;
    private Mock<ITeamValidationService> _mockTeamValidationService = null!;
    private Mock<ITournamentValidationService> _mockTournamentValidationService = null!;
    private Mock<IPlayerRepository> _mockPlayerRepository = null!;
    private Mock<ICacheInvalidationService> _mockCacheInvalidationService = null!;
    private Mock<HybridCache> _mockCache = null!;
    private ITeamService _teamService = null!;

    [SetUp]
    public void Setup()
    {
        _mockTeamRepository = new Mock<ITeamRepository>();
        _mockTournamentTeamRepository = new Mock<ITournamentTeamRepository>();
        _mockImageService = new Mock<IImageService>();
        _mockPlayerService = new Mock<IPlayerService>();
        _mockTeamValidationService = new Mock<ITeamValidationService>();
        _mockTournamentValidationService = new Mock<ITournamentValidationService>();
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _mockCacheInvalidationService = new Mock<ICacheInvalidationService>();
        _mockCache = new Mock<HybridCache>();

        _teamService = new TeamService(
            _mockTeamRepository.Object,
            _mockTournamentTeamRepository.Object,
            _mockImageService.Object,
            _mockPlayerService.Object,
            _mockTeamValidationService.Object,
            _mockTournamentValidationService.Object,
            _mockPlayerRepository.Object,
            _mockCacheInvalidationService.Object,
            _mockCache.Object
        );
    }

    [Test]
    public async Task GIVEN_ValidCreateDto_WHEN_CreateAsync_THEN_CreatesTeamSuccessfully()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var createDto = new CreateTeamDto
        {
            Name = "Phoenix Gaming",
            ShortName = "PHX"
        };

        _mockTeamRepository
            .Setup(x => x.GetByUserAsync(managerId))
            .ReturnsAsync(new List<Team>());

        _mockTeamValidationService
            .Setup(x => x.ValidateUserTeamNameUniqueness(createDto.Name, managerId))
            .Returns(Task.CompletedTask);

        _mockTeamRepository
            .Setup(x => x.CreateAsync(It.IsAny<Team>()))
            .ReturnsAsync((Team t) => t);

        _mockTeamRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Team>()))
            .ReturnsAsync((Team t) => t);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTeamRelatedCacheAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.CreateAsync(createDto, managerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Phoenix Gaming"));
        Assert.That(result.ShortName, Is.EqualTo("PHX"));
        _mockTeamRepository.Verify(x => x.CreateAsync(It.IsAny<Team>()), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTeamRelatedCacheAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GIVEN_MaxTeamsReached_WHEN_CreateAsync_THEN_ThrowsForbiddenException()
    {
        // Arrange
        var managerId = Guid.NewGuid();
        var createDto = new CreateTeamDto
        {
            Name = "New Team",
            ShortName = "NEW"
        };

        var existingTeams = Enumerable.Range(1, 5).Select(i => new Team
        {
            Id = Guid.NewGuid(),
            Name = $"Team {i}",
            UserId = managerId
        }).ToList();

        _mockTeamRepository
            .Setup(x => x.GetByUserAsync(managerId))
            .ReturnsAsync(existingTeams);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ForbiddenException>(
            async () => await _teamService.CreateAsync(createDto, managerId));

        Assert.That(ex!.Message, Does.Contain("cannot create more than 5 teams"));
    }

    [Test]
    public async Task GIVEN_ValidUpdateDto_WHEN_UpdateAsync_THEN_UpdatesTeamSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTeam = new Team
        {
            Id = teamId,
            Name = "Old Team Name",
            ShortName = "OLD",
            UserId = userId
        };

        var updateDto = new UpdateTeamDto
        {
            Name = "Updated Team Name",
            ShortName = "UPD"
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(existingTeam);

        _mockTeamValidationService
            .Setup(x => x.ValidateUserTeamNameUniqueness(updateDto.Name, userId))
            .Returns(Task.CompletedTask);

        _mockTournamentTeamRepository
            .Setup(x => x.GetTournamentsByTeamAsync(teamId))
            .ReturnsAsync(new List<Tournament>());

        _mockTeamRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Team>()))
            .ReturnsAsync((Team t) => t);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTeamRelatedCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.UpdateAsync(teamId, updateDto, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Updated Team Name"));
        Assert.That(result.ShortName, Is.EqualTo("UPD"));
        _mockTeamRepository.Verify(x => x.UpdateAsync(It.IsAny<Team>()), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTeamRelatedCacheAsync(teamId), Times.Once);
    }

    [Test]
    public void GIVEN_DuplicateNameInActiveTournament_WHEN_UpdateAsync_THEN_ThrowsConflictException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTeam = new Team
        {
            Id = teamId,
            Name = "Old Team Name",
            UserId = userId
        };

        var updateDto = new UpdateTeamDto
        {
            Name = "Duplicate Name",
            ShortName = "DUP"
        };

        var activeTournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Active Tournament",
            Status = (int)TournamentStatus.RegistrationOpen,
            StartDate = DateTime.UtcNow.AddDays(30),
            RegistrationStartDate = DateTime.UtcNow.AddDays(-5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20)
        };

        var tournamentTeams = new List<TournamentTeam>
        {
            new TournamentTeam
            {
                TeamId = Guid.NewGuid(),
                TournamentId = activeTournament.Id,
                Team = new Team { Name = "Duplicate Name" }
            }
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(existingTeam);

        _mockTeamValidationService
            .Setup(x => x.ValidateUserTeamNameUniqueness(updateDto.Name, userId))
            .Returns(Task.CompletedTask);

        _mockTournamentTeamRepository
            .Setup(x => x.GetTournamentsByTeamAsync(teamId))
            .ReturnsAsync(new List<Tournament> { activeTournament });

        _mockTournamentTeamRepository
            .Setup(x => x.GetByTournamentAsync(activeTournament.Id))
            .ReturnsAsync(tournamentTeams);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ConflictException>(
            async () => await _teamService.UpdateAsync(teamId, updateDto, userId));

        Assert.That(ex!.Message, Does.Contain("A team with this name is already registered"));
    }

    [Test]
    public async Task GIVEN_ValidTeamId_WHEN_DeleteAsync_THEN_DeletesTeamSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Team to Delete",
            UserId = userId
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateTeamCanBeDeleted(teamId, userId))
            .ReturnsAsync(team);

        _mockTeamRepository
            .Setup(x => x.DeleteAsync(teamId))
            .Returns(Task.CompletedTask);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTeamRelatedCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        await _teamService.DeleteAsync(teamId, userId);

        // Assert
        _mockTeamRepository.Verify(x => x.DeleteAsync(teamId), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTeamRelatedCacheAsync(teamId), Times.Once);
    }

    [Test]
    public async Task GIVEN_ValidPlayerDto_WHEN_AddPlayerToTeamAsync_THEN_AddsPlayerSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            UserId = userId
        };

        var playerDto = new CreatePlayerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Position = PlayerPosition.Midfielder,
            TeamId = teamId
        };

        var createdPlayer = new PlayerDto
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Position = PlayerPosition.Midfielder,
            TeamId = teamId,
            TeamName = "Test Team",
            JoinedAt = DateTime.UtcNow
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(team);

        _mockPlayerRepository
            .Setup(x => x.GetPlayerCountByTeamAsync(teamId))
            .ReturnsAsync(5);

        _mockPlayerService
            .Setup(x => x.CreateAsync(playerDto))
            .ReturnsAsync(createdPlayer);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTeamCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.AddPlayerToTeamAsync(teamId, playerDto, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo("John"));
        Assert.That(result.Position, Is.EqualTo(PlayerPosition.Midfielder));
        _mockPlayerService.Verify(x => x.CreateAsync(playerDto), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTeamCacheAsync(teamId), Times.Once);
    }

    [Test]
    public void GIVEN_MaxPlayersReached_WHEN_AddPlayerToTeamAsync_THEN_ThrowsValidationException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            UserId = userId
        };

        var playerDto = new CreatePlayerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Position = PlayerPosition.Defender,
            TeamId = teamId
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(team);

        _mockPlayerRepository
            .Setup(x => x.GetPlayerCountByTeamAsync(teamId))
            .ReturnsAsync(15);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(
            async () => await _teamService.AddPlayerToTeamAsync(teamId, playerDto, userId));

        Assert.That(ex!.Message, Does.Contain("maximum number of players (15)"));
    }

    [Test]
    public void GIVEN_MismatchedTeamId_WHEN_AddPlayerToTeamAsync_THEN_ThrowsValidationException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var differentTeamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            UserId = userId
        };

        var playerDto = new CreatePlayerDto
        {
            FirstName = "John",
            LastName = "Doe",
            Position = PlayerPosition.Defender,
            TeamId = differentTeamId
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(team);

        _mockPlayerRepository
            .Setup(x => x.GetPlayerCountByTeamAsync(teamId))
            .ReturnsAsync(5);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(
            async () => await _teamService.AddPlayerToTeamAsync(teamId, playerDto, userId));

        Assert.That(ex!.Message, Does.Contain("Player's TeamId does not match"));
    }

    [Test]
    public async Task GIVEN_ValidPlayerId_WHEN_UpdateTeamPlayerAsync_THEN_UpdatesPlayerSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            UserId = userId
        };

        var updateDto = new UpdatePlayerDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Position = PlayerPosition.Forward
        };

        var updatedPlayer = new PlayerDto
        {
            Id = playerId,
            FirstName = "Jane",
            LastName = "Smith",
            Position = PlayerPosition.Forward,
            TeamId = teamId,
            TeamName = "Test Team",
            JoinedAt = DateTime.UtcNow
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(team);

        _mockPlayerService
            .Setup(x => x.UpdateAsync(updateDto, playerId))
            .ReturnsAsync(updatedPlayer);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTeamCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.UpdateTeamPlayerAsync(teamId, playerId, updateDto, userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo("Jane"));
        Assert.That(result.Position, Is.EqualTo(PlayerPosition.Forward));
        _mockPlayerService.Verify(x => x.UpdateAsync(updateDto, playerId), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTeamCacheAsync(teamId), Times.Once);
    }

    [Test]
    public async Task GIVEN_ValidPlayerId_WHEN_RemovePlayerFromTeamAsync_THEN_RemovesPlayerSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var playerId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            UserId = userId
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(team);

        _mockPlayerService
            .Setup(x => x.DeleteAsync(teamId, playerId))
            .Returns(Task.CompletedTask);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTeamCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        await _teamService.RemovePlayerFromTeamAsync(teamId, playerId, userId);

        // Assert
        _mockPlayerService.Verify(x => x.DeleteAsync(teamId, playerId), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTeamCacheAsync(teamId), Times.Once);
    }
}
