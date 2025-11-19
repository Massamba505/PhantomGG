using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Moq;
using PhantomGG.Common.Enums;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Domain.Tournaments.Implementations;
using PhantomGG.Service.Domain.Tournaments.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Infrastructure.Email.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class TournamentServiceTests
{
    private Mock<ITournamentRepository> _mockTournamentRepository = null!;
    private Mock<IImageService> _mockImageService = null!;
    private Mock<ITournamentValidationService> _mockValidationService = null!;
    private Mock<ICacheInvalidationService> _mockCacheInvalidationService = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IEmailService> _mockEmailService = null!;
    private Mock<HybridCache> _mockCache = null!;
    private Mock<ILogger<TournamentService>> _mockLogger = null!;
    private ITournamentService _tournamentService = null!;

    [SetUp]
    public void Setup()
    {
        _mockTournamentRepository = new Mock<ITournamentRepository>();
        _mockImageService = new Mock<IImageService>();
        _mockValidationService = new Mock<ITournamentValidationService>();
        _mockCacheInvalidationService = new Mock<ICacheInvalidationService>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockEmailService = new Mock<IEmailService>();
        _mockCache = new Mock<HybridCache>();
        _mockLogger = new Mock<ILogger<TournamentService>>();

        _tournamentService = new TournamentService(
            _mockTournamentRepository.Object,
            _mockImageService.Object,
            _mockValidationService.Object,
            _mockCacheInvalidationService.Object,
            _mockUserRepository.Object,
            _mockEmailService.Object,
            _mockCache.Object,
            _mockLogger.Object
        );
    }

    [Test]
    public async Task GIVEN_ValidTournamentId_WHEN_GetByIdAsync_THEN_ReturnsTournamentDto()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();
        var tournament = new Tournament
        {
            Id = tournamentId,
            Name = "Championship 2025",
            Description = "Annual championship",
            Location = "New York",
            OrganizerId = organizerId,
            Status = (int)TournamentStatus.RegistrationOpen,
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(35),
            RegistrationStartDate = DateTime.UtcNow.AddDays(-5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20),
            MinTeams = 4,
            MaxTeams = 16,
            IsPublic = true
        };

        _mockTournamentRepository
            .Setup(x => x.GetByIdAsync(tournamentId))
            .ReturnsAsync(tournament);

        // Act
        var result = await _tournamentService.GetByIdAsync(tournamentId, organizerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(tournamentId));
        Assert.That(result.Name, Is.EqualTo("Championship 2025"));
        _mockTournamentRepository.Verify(x => x.GetByIdAsync(tournamentId), Times.Once);
    }

    [Test]
    public void GIVEN_NonExistentTournamentId_WHEN_GetByIdAsync_THEN_ThrowsNotFoundException()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();

        _mockTournamentRepository
            .Setup(x => x.GetByIdAsync(tournamentId))
            .ReturnsAsync((Tournament?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<NotFoundException>(
            async () => await _tournamentService.GetByIdAsync(tournamentId));

        Assert.That(ex!.Message, Is.EqualTo("Tournament not found"));
    }

    [Test]
    public void GIVEN_DraftTournamentAndWrongUser_WHEN_GetByIdAsync_THEN_ThrowsForbiddenException()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();
        var differentUserId = Guid.NewGuid();
        var tournament = new Tournament
        {
            Id = tournamentId,
            Name = "Draft Tournament",
            OrganizerId = organizerId,
            Status = (int)TournamentStatus.Draft,
            StartDate = DateTime.UtcNow.AddDays(30),
            RegistrationStartDate = DateTime.UtcNow.AddDays(-5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20)
        };

        _mockTournamentRepository
            .Setup(x => x.GetByIdAsync(tournamentId))
            .ReturnsAsync(tournament);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ForbiddenException>(
            async () => await _tournamentService.GetByIdAsync(tournamentId, differentUserId));

        Assert.That(ex!.Message, Is.EqualTo("Draft tournaments are only visible to their organizers"));
    }

    [Test]
    public async Task GIVEN_ValidCreateDto_WHEN_CreateAsync_THEN_CreatesTournamentSuccessfully()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var createDto = new CreateTournamentDto
        {
            Name = "New Tournament",
            Description = "Test tournament",
            Location = "Los Angeles",
            RegistrationStartDate = DateTime.UtcNow.AddDays(5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(35),
            MinTeams = 4,
            MaxTeams = 16,
            IsPublic = true
        };

        _mockValidationService
            .Setup(x => x.ValidateTournamentDatesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        _mockTournamentRepository
            .Setup(x => x.GetByOrganizerAsync(organizerId))
            .ReturnsAsync(new List<Tournament>());

        _mockTournamentRepository
            .Setup(x => x.CreateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament t) => t);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTournamentRelatedCacheAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tournamentService.CreateAsync(createDto, organizerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("New Tournament"));
        Assert.That(result.Location, Is.EqualTo("Los Angeles"));
        _mockTournamentRepository.Verify(x => x.CreateAsync(It.IsAny<Tournament>()), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTournamentRelatedCacheAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Test]
    public void GIVEN_MaxActiveTournamentsReached_WHEN_CreateAsync_THEN_ThrowsForbiddenException()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var createDto = new CreateTournamentDto
        {
            Name = "New Tournament",
            Description = "Test tournament",
            Location = "Los Angeles",
            RegistrationStartDate = DateTime.UtcNow.AddDays(5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20),
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(35),
            MinTeams = 4,
            MaxTeams = 16
        };

        _mockValidationService
            .Setup(x => x.ValidateTournamentDatesAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(Task.CompletedTask);

        var activeTournaments = Enumerable.Range(1, 10).Select(i => new Tournament
        {
            Id = Guid.NewGuid(),
            Name = $"Tournament {i}",
            OrganizerId = organizerId,
            Status = (int)TournamentStatus.RegistrationOpen,
            StartDate = DateTime.UtcNow.AddDays(30),
            RegistrationStartDate = DateTime.UtcNow.AddDays(-5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20)
        }).ToList();

        _mockTournamentRepository
            .Setup(x => x.GetByOrganizerAsync(organizerId))
            .ReturnsAsync(activeTournaments);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ForbiddenException>(
            async () => await _tournamentService.CreateAsync(createDto, organizerId));

        Assert.That(ex!.Message, Does.Contain("cannot create more than 10 active tournaments"));
    }

    [Test]
    public async Task GIVEN_ValidUpdateDto_WHEN_UpdateAsync_THEN_UpdatesTournamentSuccessfully()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();
        var existingTournament = new Tournament
        {
            Id = tournamentId,
            Name = "Old Name",
            Description = "Old description",
            Location = "Old Location",
            OrganizerId = organizerId,
            Status = (int)TournamentStatus.Draft,
            StartDate = DateTime.UtcNow.AddDays(30),
            RegistrationStartDate = DateTime.UtcNow.AddDays(-5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20)
        };

        var updateDto = new UpdateTournamentDto
        {
            Name = "Updated Name",
            Description = "Updated description",
            Location = "Updated Location"
        };

        _mockValidationService
            .Setup(x => x.ValidateCanUpdateAsync(tournamentId, organizerId))
            .ReturnsAsync(existingTournament);

        _mockTournamentRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync((Tournament t) => t);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTournamentRelatedCacheAsync(tournamentId))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tournamentService.UpdateAsync(tournamentId, updateDto, organizerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Name, Is.EqualTo("Updated Name"));
        Assert.That(result.Description, Is.EqualTo("Updated description"));
        Assert.That(result.Location, Is.EqualTo("Updated Location"));
        _mockTournamentRepository.Verify(x => x.UpdateAsync(It.IsAny<Tournament>()), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTournamentRelatedCacheAsync(tournamentId), Times.Once);
    }

    [Test]
    public async Task GIVEN_ValidTournamentId_WHEN_DeleteAsync_THEN_DeletesTournamentSuccessfully()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();
        var tournament = new Tournament
        {
            Id = tournamentId,
            Name = "Tournament to Delete",
            OrganizerId = organizerId,
            Status = (int)TournamentStatus.Draft,
            StartDate = DateTime.UtcNow.AddDays(30),
            RegistrationStartDate = DateTime.UtcNow.AddDays(-5),
            RegistrationDeadline = DateTime.UtcNow.AddDays(20)
        };

        _mockValidationService
            .Setup(x => x.ValidateCanDeleteAsync(tournamentId, organizerId))
            .ReturnsAsync(tournament);

        _mockTournamentRepository
            .Setup(x => x.DeleteAsync(tournamentId))
            .Returns(Task.CompletedTask);

        _mockCacheInvalidationService
            .Setup(x => x.InvalidateTournamentRelatedCacheAsync(tournamentId))
            .Returns(Task.CompletedTask);

        // Act
        await _tournamentService.DeleteAsync(tournamentId, organizerId);

        // Assert
        _mockTournamentRepository.Verify(x => x.DeleteAsync(tournamentId), Times.Once);
        _mockCacheInvalidationService.Verify(x => x.InvalidateTournamentRelatedCacheAsync(tournamentId), Times.Once);
    }

    [Test]
    public async Task GIVEN_ValidOrganizerId_WHEN_GetByOrganizerAsync_THEN_ReturnsOrganizerTournaments()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var tournaments = new List<Tournament>
        {
            new Tournament
            {
                Id = Guid.NewGuid(),
                Name = "Tournament 1",
                OrganizerId = organizerId,
                Status = (int)TournamentStatus.RegistrationOpen,
                StartDate = DateTime.UtcNow.AddDays(30),
                RegistrationStartDate = DateTime.UtcNow.AddDays(-5),
                RegistrationDeadline = DateTime.UtcNow.AddDays(20)
            },
            new Tournament
            {
                Id = Guid.NewGuid(),
                Name = "Tournament 2",
                OrganizerId = organizerId,
                Status = (int)TournamentStatus.InProgress,
                StartDate = DateTime.UtcNow.AddDays(-5),
                RegistrationStartDate = DateTime.UtcNow.AddDays(-20),
                RegistrationDeadline = DateTime.UtcNow.AddDays(-10)
            }
        };

        _mockTournamentRepository
            .Setup(x => x.GetByOrganizerAsync(organizerId))
            .ReturnsAsync(tournaments);

        // Act
        var result = await _tournamentService.GetByOrganizerAsync(organizerId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
        _mockTournamentRepository.Verify(x => x.GetByOrganizerAsync(organizerId), Times.Once);
    }
}
