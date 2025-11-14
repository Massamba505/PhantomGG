using NUnit.Framework;
using FluentAssertions;
using PhantomGG.Repository.Entities;
using PhantomGG.Models.DTOs.Team;
using PhantomGG.Models.DTOs.Player;
using PhantomGG.Models.DTOs.Match;
using PhantomGG.Models.DTOs.Tournament;
using PhantomGG.Models.DTOs.User;
using PhantomGG.Models.DTOs.MatchEvent;
using PhantomGG.Service.Mappings;
using PhantomGG.Common.Enums;

namespace PhantomGG.UnitTests.Mappings;

[TestFixture]
public class MappingTests
{
    [Test]
    public void TeamToDto_ValidTeam_ReturnsMappedDto()
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = "Test Team",
            ShortName = "TT",
            LogoUrl = "logo.png",
            UserId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            User = new User { FirstName = "John", LastName = "Doe", Email = "john@test.com", PasswordHash = "hash", Role = (int)UserRoles.User }
        };
        var dto = team.ToDto();
        dto.Id.Should().Be(team.Id);
        dto.Name.Should().Be("Test Team");
    }

    [Test]
    public void CreateTeamDtoToEntity_ValidDto_ReturnsMappedEntity()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateTeamDto { Name = "New Team", ShortName = "NT" };
        var entity = dto.ToEntity(userId);
        entity.Name.Should().Be("New Team");
        entity.UserId.Should().Be(userId);
    }

    [Test]
    public void PlayerToDto_ValidPlayer_ReturnsMappedDto()
    {
        var player = new Player
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Position = (int)PlayerPosition.Forward,
            TeamId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            Team = new Team { Name = "Test Team", ShortName = "TT", UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
        };
        var dto = player.ToDto();
        dto.FirstName.Should().Be("John");
    }

    [Test]
    public void CreatePlayerDtoToEntity_ValidDto_ReturnsMappedEntity()
    {
        var dto = new CreatePlayerDto { FirstName = "Jane", LastName = "Smith", Position = PlayerPosition.Midfielder, TeamId = Guid.NewGuid() };
        var entity = dto.ToEntity();
        entity.FirstName.Should().Be("Jane");
    }

    [Test]
    public void MatchToDto_ValidMatch_ReturnsMappedDto()
    {
        var match = new Repository.Entities.Match
        {
            Id = Guid.NewGuid(),
            TournamentId = Guid.NewGuid(),
            HomeTeamId = Guid.NewGuid(),
            AwayTeamId = Guid.NewGuid(),
            MatchDate = DateTime.UtcNow,
            Status = (int)MatchStatus.Completed,
            HomeScore = 2,
            AwayScore = 1,
            HomeTeam = new Team { Name = "Home", ShortName = "HT", UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
            AwayTeam = new Team { Name = "Away", ShortName = "AT", UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
        };
        var dto = match.ToDto();
        dto.Id.Should().Be(match.Id);
    }

    [Test]
    public void CreateMatchDtoToEntity_ValidDto_ReturnsMappedEntity()
    {
        var dto = new CreateMatchDto { TournamentId = Guid.NewGuid(), HomeTeamId = Guid.NewGuid(), AwayTeamId = Guid.NewGuid(), MatchDate = DateTime.UtcNow };
        var entity = dto.ToEntity();
        entity.TournamentId.Should().Be(dto.TournamentId);
    }

    [Test]
    public void TournamentToDto_ValidTournament_ReturnsMappedDto()
    {
        var tournament = new Tournament
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            MaxTeams = 16,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(30),
            Status = (int)TournamentStatus.InProgress,
            OrganizerId = Guid.NewGuid(),
            Organizer = new User { FirstName = "Org", LastName = "User", Email = "org@test.com", PasswordHash = "hash", Role = (int)UserRoles.Organizer }
        };
        var dto = tournament.ToDto();
        dto.Id.Should().Be(tournament.Id);
    }

    [Test]
    public void CreateTournamentDtoToEntity_ValidDto_ReturnsMappedEntity()
    {
        var dto = new CreateTournamentDto { Name = "New", MaxTeams = 16, StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30) };
        var entity = dto.ToEntity(Guid.NewGuid());
        entity.Name.Should().Be("New");
    }

    [Test]
    public void UserToDto_ValidUser_ReturnsMappedDto()
    {
        var user = new User { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Email = "john@test.com", PasswordHash = "hash", Role = (int)UserRoles.User };
        var dto = user.ToDto();
        dto.Id.Should().Be(user.Id);
    }

    [Test]
    public void MatchEventToDto_ValidMatchEvent_ReturnsMappedDto()
    {
        var matchEvent = new MatchEvent
        {
            Id = Guid.NewGuid(),
            MatchId = Guid.NewGuid(),
            PlayerId = Guid.NewGuid(),
            TeamId = Guid.NewGuid(),
            EventType = (int)MatchEventType.Goal,
            Minute = 45,
            Player = new Player { FirstName = "Goal", LastName = "Scorer", Position = (int)PlayerPosition.Forward, TeamId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
            Team = new Team { Name = "Test Team", ShortName = "TT", UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow }
        };
        var dto = matchEvent.ToDto();
        dto.Id.Should().Be(matchEvent.Id);
    }

    [Test]
    public void CreateMatchEventDtoToEntity_ValidDto_ReturnsMappedEntity()
    {
        var dto = new CreateMatchEventDto { MatchId = Guid.NewGuid(), PlayerId = Guid.NewGuid(), TeamId = Guid.NewGuid(), EventType = MatchEventType.YellowCard, Minute = 30 };
        var entity = dto.ToEntity();
        entity.EventType.Should().Be((int)MatchEventType.YellowCard);
    }

    [Test]
    public void OrganizerDto_FromUser_MapsCorrectly()
    {
        var user = new User { Id = Guid.NewGuid(), FirstName = "Org", LastName = "User", Email = "org@test.com", PasswordHash = "hash", Role = (int)UserRoles.Organizer, ProfilePictureUrl = "" };
        var dto = user.ToOrganizerDto();
        dto.FirstName.Should().Be("Org");
    }

    [Test]
    public void TeamDto_WithNullProperties_HandlesGracefully()
    {
        var team = new Team { Id = Guid.NewGuid(), Name = "Test", ShortName = "TT", UserId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow, User = null };
        var dto = team.ToDto();
        dto.Name.Should().Be("Test");
    }
}
