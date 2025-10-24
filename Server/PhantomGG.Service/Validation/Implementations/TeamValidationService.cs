using PhantomGG.Common.Enums;
using PhantomGG.Repository.Entities;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Service.Exceptions;
using PhantomGG.Service.Validation.Interfaces;

namespace PhantomGG.Service.Validation.Implementations;

public class TeamValidationService(
    ITeamRepository teamRepository,
    ITournamentTeamRepository tournamentTeamRepository) : ITeamValidationService
{
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentTeamRepository _tournamentTeamRepository = tournamentTeamRepository;

    public async Task<Team> ValidateTeamExistsAsync(Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(userId);
        if (team == null)
            throw new NotFoundException($"Team not found");

        return team;
    }

    public async Task<Team> ValidateCanManageTeamAsync(Guid userId, Guid teamId)
    {
        var team = await ValidateTeamExistsAsync(teamId);

        if (team.UserId != userId)
            throw new ForbiddenException("You don't have permission to manage this teams");

        return team;
    }

    public async Task<Team> ValidateTeamCanBeDeleted(Guid teamId, Guid userId)
    {
        var team = await ValidateCanManageTeamAsync(userId, teamId);
        var tournaments = await _tournamentTeamRepository.GetTournamentsByTeamAsync(team.Id);
        var activeTournaments = tournaments.Where(t => t.Status != (int)TournamentStatus.Completed).ToList();

        if (activeTournaments.Any())
        {
            var tournamentNames = string.Join(", ", activeTournaments.Select(t => t.Name));
            throw new ForbiddenException($"Cannot delete team. Team is registered in tournaments: {tournamentNames}");
        }

        return team;
    }

    public async Task ValidateUserTeamNameUniqueness(string teamName, Guid managerId)
    {
        var existingTeams = await _teamRepository.GetByUserAsync(managerId);
        if (existingTeams.Any(t => t.Name.Equals(teamName, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ConflictException("A team with this name is already available");
        }
    }
}
