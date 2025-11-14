using PhantomGG.Common.Enums;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Implementations;
using PhantomGG.UnitTests.Helpers;

namespace PhantomGG.UnitTests.Repositories;

[TestFixture]
public class PlayerRepositoryTests
{
    private PlayerRepository _playerRepository = null!;
    private PhantomContext _context = null!;

    [SetUp]
    public void Setup()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _playerRepository = new PlayerRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GIVEN_ValidPlayer_WHEN_CreateAsync_THEN_CreatesPlayerSuccessfully()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            ShortName = "TT",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();

        var player = new Player
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Position = (int)PlayerPosition.Midfielder,
            TeamId = teamId,
            Email = "john.doe@example.com",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var result = await _playerRepository.CreateAsync(player);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(player.Id));
        Assert.That(result.FirstName, Is.EqualTo("John"));

        var dbPlayer = await _context.Players.FindAsync(player.Id);
        Assert.That(dbPlayer, Is.Not.Null);
    }

    [Test]
    public async Task GIVEN_ValidPlayerId_WHEN_GetByIdAsync_THEN_ReturnsPlayer()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            ShortName = "TT",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        await _context.Teams.AddAsync(team);

        var playerId = Guid.NewGuid();
        var player = new Player
        {
            Id = playerId,
            FirstName = "Jane",
            LastName = "Smith",
            Position = (int)PlayerPosition.Forward,
            TeamId = teamId,
            CreatedAt = DateTime.UtcNow
        };
        await _context.Players.AddAsync(player);
        await _context.SaveChangesAsync();

        // Act
        var result = await _playerRepository.GetByIdAsync(playerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(playerId));
        Assert.That(result.FirstName, Is.EqualTo("Jane"));
        Assert.That(result.Team, Is.Not.Null);
    }

    [Test]
    public async Task GIVEN_ValidTeamId_WHEN_GetByTeamAsync_THEN_ReturnsPlayers()
    {
        // Arrange
        var teamId = Guid.NewGuid();
        var team = new Team
        {
            Id = teamId,
            Name = "Test Team",
            ShortName = "TT",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        await _context.Teams.AddAsync(team);

        var player1 = new Player
        {
            Id = Guid.NewGuid(),
            FirstName = "Alice",
            LastName = "Johnson",
            Position = (int)PlayerPosition.Midfielder,
            TeamId = teamId,
            CreatedAt = DateTime.UtcNow
        };

        var player2 = new Player
        {
            Id = Guid.NewGuid(),
            FirstName = "Bob",
            LastName = "Smith",
            Position = (int)PlayerPosition.Defender,
            TeamId = teamId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Players.AddRangeAsync(player1, player2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _playerRepository.GetByTeamAsync(teamId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }
}
