using PhantomGG.API.DTOs.Team;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class TeamService : ITeamService
{
    private readonly ITeamRepository _teamRepository;
    private readonly ITournamentRepository _tournamentRepository;

    public TeamService(ITeamRepository teamRepository, ITournamentRepository tournamentRepository)
    {
        _teamRepository = teamRepository;
        _tournamentRepository = tournamentRepository;
    }

    public async Task<IEnumerable<TeamDto>> GetAllAsync()
    {
        var teams = await _teamRepository.GetAllAsync();
        return teams.Select(MapToDto);
    }

    public async Task<TeamDto> GetByIdAsync(Guid id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null)
            throw new NotFoundException("Team not found");

        return MapToDto(team);
    }

    public async Task<IEnumerable<TeamDto>> GetByLeaderAsync(Guid leaderId)
    {
        // Note: Since Team model uses Manager (string), we can't directly filter by Guid
        // This method might need to be reconsidered based on your authentication system
        var teams = await _teamRepository.GetAllAsync();
        return teams.Select(MapToDto);
    }

    public async Task<IEnumerable<TeamDto>> GetByTournamentAsync(Guid tournamentId)
    {
        var teams = await _teamRepository.GetByTournamentAsync(tournamentId);
        return teams.Select(MapToDto);
    }

    public async Task<IEnumerable<TeamDto>> SearchAsync(TeamSearchDto searchDto)
    {
        var teams = await _teamRepository.SearchAsync(searchDto);
        return teams.Select(MapToDto);
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid leaderId)
    {
        // Validate tournament exists
        if (!await _tournamentRepository.ExistsAsync(createDto.TournamentId))
            throw new NotFoundException("Tournament not found");

        // Check if team name is unique in tournament
        if (await _teamRepository.TeamNameExistsInTournamentAsync(createDto.Name, createDto.TournamentId))
            throw new ConflictException("A team with this name already exists in the tournament");

        var team = MapFromCreateDto(createDto);
        var createdTeam = await _teamRepository.CreateAsync(team);
        return MapToDto(createdTeam);
    }

    public async Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto updateDto, Guid userId)
    {
        if (!await _teamRepository.ExistsAsync(id))
            throw new NotFoundException("Team not found");

        var existingTeam = await _teamRepository.GetByIdAsync(id);
        if (existingTeam == null)
            throw new NotFoundException("Team not found");

        // Check if new name is unique in tournament (excluding current team)
        if (await _teamRepository.TeamNameExistsInTournamentAsync(updateDto.Name, existingTeam.TournamentId, id))
            throw new ConflictException("A team with this name already exists in the tournament");

        UpdateTeamFromDto(existingTeam, updateDto);
        var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);
        return MapToDto(updatedTeam);
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        if (!await _teamRepository.ExistsAsync(id))
            throw new NotFoundException("Team not found");

        await _teamRepository.DeleteAsync(id);
    }

    private static TeamDto MapToDto(Team team)
    {
        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            Manager = team.Manager,
            NumberOfPlayers = team.NumberOfPlayers,
            LogoUrl = team.LogoUrl,
            TournamentId = team.TournamentId,
            TournamentName = team.Tournament?.Name ?? "Unknown",
            CreatedAt = team.CreatedAt,
            UpdatedAt = team.UpdatedAt
        };
    }

    private static Team MapFromCreateDto(CreateTeamDto dto)
    {
        return new Team
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Manager = dto.Manager,
            NumberOfPlayers = dto.NumberOfPlayers,
            LogoUrl = dto.LogoUrl,
            TournamentId = dto.TournamentId,
            IsActive = true
        };
    }

    private static void UpdateTeamFromDto(Team team, UpdateTeamDto dto)
    {
        team.Name = dto.Name;
        team.Manager = dto.Manager;
        team.NumberOfPlayers = dto.NumberOfPlayers;
        team.LogoUrl = dto.LogoUrl;
        team.UpdatedAt = DateTime.UtcNow;
    }
}
