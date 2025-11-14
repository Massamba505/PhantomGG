using NUnit.Framework;
using FluentAssertions;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Implementations;
using PhantomGG.UnitTests.Helpers;

namespace PhantomGG.UnitTests.Repositories;

[TestFixture]
public class TeamRepositoryTests
{
    private PhantomContext _context;
    private TeamRepository _repository;

    [SetUp]
    public void Setup()
    {
        _context = TestDbContextFactory.CreateInMemoryContext();
        _repository = new TeamRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task CreateAsync_ValidTeam_ReturnsCreatedTeam()
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = "Test Team",
            ShortName = "TT",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };

        var result = await _repository.CreateAsync(team);

        result.Should().NotBeNull();
        result.Id.Should().Be(team.Id);
        result.Name.Should().Be("Test Team");
    }

    [Test]
    public async Task GetByIdAsync_ExistingTeam_ReturnsTeam()
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = "Test Team",
            ShortName = "TT",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateAsync(team);

        var result = await _repository.GetByIdAsync(team.Id);

        result.Should().NotBeNull();
        result.Name.Should().Be("Test Team");
    }

    [Test]
    public async Task UpdateAsync_ExistingTeam_ReturnsUpdatedTeam()
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = "Original",
            ShortName = "OR",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateAsync(team);

        team.Name = "Updated";
        var result = await _repository.UpdateAsync(team);

        result.Name.Should().Be("Updated");
    }

    [Test]
    public async Task DeleteAsync_ExistingTeam_RemovesTeam()
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = "Test Team",
            ShortName = "TT",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow
        };
        await _repository.CreateAsync(team);

        await _repository.DeleteAsync(team.Id);

        var result = await _repository.GetByIdAsync(team.Id);
        result.Should().BeNull();
    }

    [Test]
    public async Task GetByUserAsync_ExistingTeams_ReturnsUserTeams()
    {
        var userId = Guid.NewGuid();
        var team1 = new Team { Id = Guid.NewGuid(), Name = "Team 1", ShortName = "T1", UserId = userId, CreatedAt = DateTime.UtcNow };
        var team2 = new Team { Id = Guid.NewGuid(), Name = "Team 2", ShortName = "T2", UserId = userId, CreatedAt = DateTime.UtcNow };
        await _repository.CreateAsync(team1);
        await _repository.CreateAsync(team2);

        var result = await _repository.GetByUserAsync(userId);

        result.Should().HaveCount(2);
    }
}
