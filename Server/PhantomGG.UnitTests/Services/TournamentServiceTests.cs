using Moq;
using NUnit.Framework;
using FluentAssertions;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using PhantomGG.Service.Domain.Tournaments.Implementations;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Entities;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;
using PhantomGG.Service.Infrastructure.Email.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Common.Enums;

namespace PhantomGG.UnitTests.Services;

[TestFixture]
public class TournamentServiceTests
{
    private Mock<ITournamentRepository> _tournamentRepositoryMock;
    private Mock<IImageService> _imageServiceMock;
    private Mock<ITournamentValidationService> _validationServiceMock;
    private Mock<ICacheInvalidationService> _cacheInvalidationServiceMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IEmailService> _emailServiceMock;
    private Mock<HybridCache> _cacheMock;
    private Mock<ILogger<TournamentService>> _loggerMock;
    private TournamentService _tournamentService;

    [SetUp]
    public void Setup()
    {
        _tournamentRepositoryMock = new Mock<ITournamentRepository>();
        _imageServiceMock = new Mock<IImageService>();
        _validationServiceMock = new Mock<ITournamentValidationService>();
        _cacheInvalidationServiceMock = new Mock<ICacheInvalidationService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _cacheMock = new Mock<HybridCache>();
        _loggerMock = new Mock<ILogger<TournamentService>>();

        _tournamentService = new TournamentService(
            _tournamentRepositoryMock.Object,
            _imageServiceMock.Object,
            _validationServiceMock.Object,
            _cacheInvalidationServiceMock.Object,
            _userRepositoryMock.Object,
            _emailServiceMock.Object,
            _cacheMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public void GetByIdAsync_TournamentNotFound_ThrowsNotFoundException()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();

        _tournamentRepositoryMock
            .Setup(x => x.GetByIdAsync(tournamentId))
            .ReturnsAsync((Tournament)null);

        // Act & Assert
        var exception = Assert.ThrowsAsync<NotFoundException>(
            async () => await _tournamentService.GetByIdAsync(tournamentId)
        );

        exception.Message.Should().Contain("Tournament not found");
    }

    [Test]
    public void GetByIdAsync_DraftTournamentNonOwner_ThrowsForbiddenException()
    {
        // Arrange
        var tournamentId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();
        var currentUserId = Guid.NewGuid(); // Different user

        var tournament = new Tournament
        {
            Id = tournamentId,
            Name = "Draft Tournament",
            OrganizerId = organizerId,
            Status = (int)TournamentStatus.Draft,
            MaxTeams = 16,
            StartDate = DateTime.UtcNow.AddDays(30)
        };

        _tournamentRepositoryMock
            .Setup(x => x.GetByIdAsync(tournamentId))
            .ReturnsAsync(tournament);

        // Act & Assert
        var exception = Assert.ThrowsAsync<ForbiddenException>(
            async () => await _tournamentService.GetByIdAsync(tournamentId, currentUserId)
        );

        exception.Message.Should().Contain("Draft tournaments are only visible to their organizers");
    }

    [Test]
    public async Task CreateAsync_ValidTournament_ReturnsTournamentDto()
    {
        // Arrange
        var organizerId = Guid.NewGuid();
        var createDto = new CreateTournamentDto
        {
            Name = "New Tournament",
            MaxTeams = 16,
            StartDate = DateTime.UtcNow.AddDays(30),
            EndDate = DateTime.UtcNow.AddDays(60),
            Location = "Test Location",
            Description = "Test Description"
        };

        var createdTournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = createDto.Name,
            MaxTeams = createDto.MaxTeams,
            StartDate = createDto.StartDate,
            EndDate = createDto.EndDate,
            Location = createDto.Location,
            Description = createDto.Description,
            OrganizerId = organizerId,
            Status = (int)TournamentStatus.Draft
        };

        _tournamentRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync(createdTournament);

        _tournamentRepositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Tournament>()))
            .ReturnsAsync(createdTournament);

        _cacheInvalidationServiceMock
            .Setup(x => x.InvalidateTournamentCacheAsync(It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tournamentService.CreateAsync(createDto, organizerId);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(createDto.Name);
        result.MaxTeams.Should().Be(createDto.MaxTeams);
        _tournamentRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Tournament>()), Times.Once);
    }
}
