using PhantomGG.API.Common;
using PhantomGG.API.DTOs.Team;
using PhantomGG.API.Exceptions;
using PhantomGG.API.Mappings;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Interfaces;

namespace PhantomGG.API.Services.Implementations;

public class TeamService(
    ITeamRepository teamRepository, 
    ITournamentRepository tournamentRepository, 
    IImageService imageService) : ITeamService
{
    private readonly ITeamRepository _teamRepository = teamRepository;
    private readonly ITournamentRepository _tournamentRepository = tournamentRepository;
    private readonly IImageService _imageService = imageService;

    public async Task<IEnumerable<TeamDto>> GetAllAsync()
    {
        var teams = await _teamRepository.GetAllAsync();
        return teams.Select(team => team.ToTeamDto());
    }

    public async Task<TeamDto> GetByIdAsync(Guid id)
    {
        var team = await _teamRepository.GetByIdAsync(id);
        if (team == null)
            throw new NotFoundException("Team not found");

        return team.ToTeamDto();
    }

    public async Task<IEnumerable<TeamDto>> GetByLeaderAsync(Guid leaderId)
    {
        var teams = await _teamRepository.GetAllAsync();
        return teams.Select(team => team.ToTeamDto());
    }

    public async Task<IEnumerable<TeamDto>> GetByTournamentAsync(Guid tournamentId)
    {
        var teams = await _teamRepository.GetByTournamentAsync(tournamentId);
        return teams.Select(team => team.ToTeamDto());
    }

    public async Task<IEnumerable<TeamDto>> SearchAsync(TeamSearchDto searchDto)
    {
        var teams = await _teamRepository.SearchAsync(searchDto);
        return teams.Select(team => team.ToTeamDto());
    }

    public async Task<TeamDto> CreateAsync(CreateTeamDto createDto, Guid leaderId)
    {
        if (!await _tournamentRepository.ExistsAsync(createDto.TournamentId))
            throw new NotFoundException("Tournament not found");

        if (await _teamRepository.TeamNameExistsInTournamentAsync(createDto.Name, createDto.TournamentId))
            throw new ConflictException("A team with this name already exists in the tournament");

        var team = createDto.ToTeam();
        var createdTeam = await _teamRepository.CreateAsync(team);
        return createdTeam.ToTeamDto();
    }

    public async Task<TeamDto> UpdateAsync(Guid id, UpdateTeamDto updateDto, Guid userId)
    {
        if (!await _teamRepository.ExistsAsync(id))
            throw new NotFoundException("Team not found");

        var existingTeam = await _teamRepository.GetByIdAsync(id);
        if (existingTeam == null)
            throw new NotFoundException("Team not found");

        if (await _teamRepository.TeamNameExistsInTournamentAsync(updateDto.Name, existingTeam.TournamentId, id))
            throw new ConflictException("A team with this name already exists in the tournament");

        existingTeam.UpdateFromDto(updateDto);
        var updatedTeam = await _teamRepository.UpdateAsync(existingTeam);
        return updatedTeam.ToTeamDto();
    }

    public async Task DeleteAsync(Guid id, Guid userId)
    {
        if (!await _teamRepository.ExistsAsync(id))
            throw new NotFoundException("Team not found");

        await _teamRepository.DeleteAsync(id);
    }

    public async Task<string> UploadTeamLogoAsync(Guid teamId, IFormFile file, Guid userId)
    {
        var team = await _teamRepository.GetByIdAsync(teamId);
        if (team == null)
        {
            throw new NotFoundException("Team not found");
        }

        if(team.Tournament.Organizer != userId)
        {
            throw new ForbiddenException("You are not authorized to modify this team");
        }

        if (!string.IsNullOrEmpty(team.LogoUrl))
        {
            await _imageService.DeleteImageAsync(team.LogoUrl);
        }

        var logoUrl = await _imageService.SaveImageAsync(file, ImageType.Logo, teamId);

        team.LogoUrl = logoUrl;
        await _teamRepository.UpdateAsync(team);

        return logoUrl;
    }
}
