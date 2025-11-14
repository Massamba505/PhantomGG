using Moq;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Domain.Teams.Implementations;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class PlayerServiceTests
{
    private Mock<IPlayerRepository> _mockPlayerRepository = null!;
    private Mock<IImageService> _mockImageService = null!;
    private Mock<IPlayerValidationService> _mockPlayerValidationService = null!;
    private Mock<ITeamValidationService> _mockTeamValidationService = null!;
    private Mock<ITeamRepository> _mockTeamRepository = null!;
    private PlayerService _playerService = null!;

    [SetUp]
    public void Setup()
    {
        _mockPlayerRepository = new Mock<IPlayerRepository>();
        _mockImageService = new Mock<IImageService>();
        _mockPlayerValidationService = new Mock<IPlayerValidationService>();
        _mockTeamValidationService = new Mock<ITeamValidationService>();
        _mockTeamRepository = new Mock<ITeamRepository>();

        _playerService = new PlayerService(
            _mockPlayerRepository.Object,
            _mockImageService.Object,
            _mockPlayerValidationService.Object,
            _mockTeamValidationService.Object,
            _mockTeamRepository.Object
        );
    }

    [Test]
    public async Task GIVEN_ValidPlayerId_WHEN_GetByIdAsync_THEN_ReturnsPlayerDto()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            FirstName = "John",
            LastName = "Doe",
            Position = (int)PlayerPosition.Midfielder,
            TeamId = teamId,
            Team = new Team { Id = teamId, Name = "Test Team" },
            CreatedAt = DateTime.UtcNow
        };

        _mockPlayerValidationService
            .Setup(x => x.ValidatePlayerExistsAsync(playerId))
            .ReturnsAsync(player);

        // Act
        var result = await _playerService.GetByIdAsync(playerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(playerId));
        Assert.That(result.FirstName, Is.EqualTo("John"));
        Assert.That(result.LastName, Is.EqualTo("Doe"));
        Assert.That(result.Position, Is.EqualTo(PlayerPosition.Midfielder));
    }

    [Test]
    public void GIVEN_NonExistentPlayer_WHEN_GetByIdAsync_THEN_ThrowsNotFoundException()
    {
        // Arrange
        var playerId = Guid.NewGuid();

        _mockPlayerValidationService
            .Setup(x => x.ValidatePlayerExistsAsync(playerId))
            .ThrowsAsync(new NotFoundException("Player not found"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(
            async () => await _playerService.GetByIdAsync(playerId));

        Assert.That(ex!.Message, Is.EqualTo("Player not found"));
    }

    [Test]
    public async Task GIVEN_ValidTeamId_WHEN_GetByTeamAsync_THEN_ReturnsPlayersList()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team { Id = teamId, Name = "Test Team" };
        var players = new List<Player>
        {
            new Player
            {
                Id = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                Position = (int)PlayerPosition.Midfielder,
                TeamId = teamId,
                Team = team,
                CreatedAt = DateTime.UtcNow
            },
            new Player
            {
                Id = Guid.NewGuid(),
                FirstName = "Jane",
                LastName = "Smith",
                Position = (int)PlayerPosition.Forward,
                TeamId = teamId,
                Team = team,
                CreatedAt = DateTime.UtcNow
            }
        };

        _mockTeamValidationService
            .Setup(x => x.ValidateTeamExistsAsync(teamId))
            .ReturnsAsync(team);

        _mockPlayerRepository
            .Setup(x => x.GetByTeamAsync(teamId))
            .ReturnsAsync(players);

        // Act
        var result = await _playerService.GetByTeamAsync(teamId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task GIVEN_ValidCreateDto_WHEN_CreateAsync_THEN_CreatesPlayerSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var createDto = new CreatePlayerDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Position = PlayerPosition.Forward,
            TeamId = teamId,
            Email = "jane.smith@example.com"
        };

        var team = new Team { Id = teamId, Name = "Test Team" };

        _mockTeamValidationService
            .Setup(x => x.ValidateTeamExistsAsync(teamId))
            .ReturnsAsync(team);

        _mockPlayerValidationService
            .Setup(x => x.ValidateMaxPlayersPerTeamAsync(teamId, 15))
            .Returns(Task.CompletedTask);

        _mockPlayerValidationService
            .Setup(x => x.ValidatePlayerPositionDistributionAsync(teamId, (int)PlayerPosition.Forward))
            .Returns(Task.CompletedTask);

        _mockPlayerValidationService
            .Setup(x => x.ValidateEmailUniquenessWithinTeamAsync("jane.smith@example.com", teamId, It.IsAny<Guid?>()))
            .Returns(Task.CompletedTask);

        _mockPlayerRepository
            .Setup(x => x.CreateAsync(It.IsAny<Player>()))
            .ReturnsAsync((Player p) =>
            {
                p.Team = team;
                return p;
            });

        // Act
        var result = await _playerService.CreateAsync(createDto);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.FirstName, Is.EqualTo("Jane"));
        Assert.That(result.LastName, Is.EqualTo("Smith"));
        Assert.That(result.Position, Is.EqualTo(PlayerPosition.Forward));
        _mockPlayerRepository.Verify(x => x.CreateAsync(It.IsAny<Player>()), Times.Once);
    }

    [Test]
    public async Task GIVEN_ValidPlayer_WHEN_DeleteAsync_THEN_DeletesPlayerSuccessfully()
    {
        // Arrange
        var playerId = Guid.NewGuid();
        var teamId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            FirstName = "John",
            LastName = "Doe",
            Position = (int)PlayerPosition.Midfielder,
            TeamId = teamId,
            PhotoUrl = string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        _mockPlayerValidationService
            .Setup(x => x.ValidatePlayerExistsAsync(playerId))
            .ReturnsAsync(player);

        _mockPlayerValidationService
            .Setup(x => x.ValidatePlayerBelongsToTeamAsync(playerId, teamId))
            .Returns(Task.CompletedTask);

        _mockPlayerValidationService
            .Setup(x => x.ValidatePlayerNotInMatchAsync(playerId))
            .Returns(Task.CompletedTask);

        _mockPlayerRepository
            .Setup(x => x.DeleteAsync(playerId))
            .Returns(Task.CompletedTask);

        // Act
        await _playerService.DeleteAsync(teamId, playerId);

        // Assert
        _mockPlayerRepository.Verify(x => x.DeleteAsync(playerId), Times.Once);
    }
}
