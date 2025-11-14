using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using PhantomGG.Service.Domain.Teams.Implementations;
using PhantomGG.Service.Domain.Teams.Interfaces;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Entities;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.DTOs;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Common.Enums;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class TeamServiceTests
{
    private Mock<ITeamRepository> _teamRepositoryMock;
    private Mock<ITournamentTeamRepository> _tournamentTeamRepositoryMock;
    private Mock<IImageService> _imageServiceMock;
    private Mock<IPlayerService> _playerServiceMock;
    private Mock<ITeamValidationService> _teamValidationServiceMock;
    private Mock<ITournamentValidationService> _tournamentValidationServiceMock;
    private Mock<IPlayerRepository> _playerRepositoryMock;
    private Mock<ICacheInvalidationService> _cacheInvalidationServiceMock;
    private Mock<HybridCache> _cacheMock;
    private TeamService _teamService;

    [SetUp]
    public void Setup()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _tournamentTeamRepositoryMock = new Mock<ITournamentTeamRepository>();
        _imageServiceMock = new Mock<IImageService>();
        _playerServiceMock = new Mock<IPlayerService>();
        _teamValidationServiceMock = new Mock<ITeamValidationService>();
        _tournamentValidationServiceMock = new Mock<ITournamentValidationService>();
        _playerRepositoryMock = new Mock<IPlayerRepository>();
        _cacheInvalidationServiceMock = new Mock<ICacheInvalidationService>();
        _cacheMock = new Mock<HybridCache>();

        _teamService = new TeamService(
            _teamRepositoryMock.Object,
            _tournamentTeamRepositoryMock.Object,
            _imageServiceMock.Object,
            _playerServiceMock.Object,
            _teamValidationServiceMock.Object,
            _tournamentValidationServiceMock.Object,
            _playerRepositoryMock.Object,
            _cacheInvalidationServiceMock.Object,
            _cacheMock.Object
        );
    }

    [Test]
    public async Task CreateAsync_ValidTeam_ReturnsCreatedTeam()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createDto = new CreateTeamDto
        {
            Name = "New Team",
            ShortName = "NT"
        };

        var existingTeams = new List<Team>();
        _teamRepositoryMock
            .Setup(x => x.GetByUserAsync(userId))
            .ReturnsAsync(existingTeams);

        _teamValidationServiceMock
            .Setup(x => x.ValidateUserTeamNameUniqueness(createDto.Name, userId))
            .Returns(Task.CompletedTask);

        var createdTeam = new Team
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            ShortName = createDto.ShortName,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _teamRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Team>()))
            .ReturnsAsync(createdTeam);

        _teamRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Team>()))
            .ReturnsAsync(createdTeam);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateTeamRelatedCacheAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.CreateAsync(createDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.ShortName.Should().Be(createDto.ShortName);
        _teamRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Team>()), Times.Once);
    }

    [Test]
    public void CreateAsync_ExceedsMaxTeams_ThrowsForbiddenException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var createDto = new CreateTeamDto
        {
            Name = "New Team",
            ShortName = "NT"
        };

        var existingTeams = Enumerable.Range(1, 5).Select(i => new Team
        {
            Id = Guid.NewGuid(),
            Name = $"Team {i}",
            ShortName = $"T{i}",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        }).ToList();

        _teamRepositoryMock
            .Setup(x => x.GetByUserAsync(userId))
            .ReturnsAsync(existingTeams);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ForbiddenException>(
            async () => await _teamService.CreateAsync(createDto, userId)
        );

        exception.Message.Should().Contain("cannot create more than 5 teams");
    }

    [Test]
    public async Task UpdateAsync_ValidUpdate_ReturnsUpdatedTeam()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var existingTeam = new Team
        {
            Id = teamId,
            Name = "Old Name",
            ShortName = "ON",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var updateDto = new UpdateTeamDto
        {
            Name = "New Name",
            ShortName = "NN"
        };

        _teamValidationServiceMock
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(existingTeam);

        _teamValidationServiceMock
            .Setup(x => x.ValidateUserTeamNameUniqueness(updateDto.Name, userId))
            .Returns(Task.CompletedTask);

        _tournamentTeamRepositoryMock
            .Setup(x => x.GetTournamentsByTeamAsync(teamId))
            .ReturnsAsync(new List<Tournament>());

        var updatedTeam = new Team
        {
            Id = teamId,
            Name = updateDto.Name,
            ShortName = updateDto.ShortName,
            UserId = userId,
            CreatedAt = existingTeam.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        _teamRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Team>()))
            .ReturnsAsync(updatedTeam);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateTeamRelatedCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.UpdateAsync(teamId, updateDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(updateDto.Name);
        _teamRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Team>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_ValidTeam_DeletesSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Team to Delete",
            ShortName = "TD",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _teamValidationServiceMock
            .Setup(x => x.ValidateTeamCanBeDeleted(teamId, userId))
            .ReturnsAsync(team);

        _teamRepositoryMock
            .Setup(x => x.DeleteAsync(teamId))
            .Returns(Task.CompletedTask);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateTeamRelatedCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        await _teamService.DeleteAsync(teamId, userId);

        // Assert
        _teamRepositoryMock.Verify(x => x.DeleteAsync(teamId), Times.Once);
        _cacheInvalidationServiceMock.Verify(x => x.InvalidateTeamRelatedCacheAsync(teamId), Times.Once);
    }

    [Test]
    public async Task AddPlayerToTeamAsync_ValidPlayer_ReturnsCreatedPlayer()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            ShortName = "TT",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var createPlayerDto = new CreatePlayerDto
        {
            TeamId = teamId,
            FirstName = "John",
            LastName = "Doe",
            Position = PlayerPosition.Forward
        };

        var createdPlayer = new PlayerDto
        {
            Id = Guid.NewGuid(),
            TeamId = teamId,
            FirstName = "John",
            LastName = "Doe",
            Position = PlayerPosition.Forward,
            TeamName = "Test Team",
            JoinedAt = DateTime.UtcNow
        };

        _teamValidationServiceMock
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(team);

        _playerRepositoryMock
            .Setup(x => x.GetPlayerCountByTeamAsync(teamId))
            .ReturnsAsync(5);

        _playerServiceMock
            .Setup(x => x.CreateAsync(createPlayerDto))
            .ReturnsAsync(createdPlayer);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateTeamCacheAsync(teamId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _teamService.AddPlayerToTeamAsync(teamId, createPlayerDto, userId);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.TeamId.Should().Be(teamId);
        _playerServiceMock.Verify(x => x.CreateAsync(createPlayerDto), Times.Once);
    }

    [Test]
    public void AddPlayerToTeamAsync_ExceedsMaxPlayers_ThrowsValidationException()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            ShortName = "TT",
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var createPlayerDto = new CreatePlayerDto
        {
            TeamId = teamId,
            FirstName = "John",
            LastName = "Doe",
            Position = PlayerPosition.Forward
        };

        _teamValidationServiceMock
            .Setup(x => x.ValidateCanManageTeamAsync(userId, teamId))
            .ReturnsAsync(team);

        _playerRepositoryMock
            .Setup(x => x.GetPlayerCountByTeamAsync(teamId))
            .ReturnsAsync(15);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ValidationException>(
            async () => await _teamService.AddPlayerToTeamAsync(teamId, createPlayerDto, userId)
        );

        exception.Message.Should().Contain("maximum number of players (15)");
    }
}
