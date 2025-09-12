using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Common;
using PhantomGG.API.Data;
using PhantomGG.API.DTOs;
using PhantomGG.API.DTOs.Tournament;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;

namespace PhantomGG.API.Repositories.Implementations;

public class TournamentRepository(PhantomContext context) : ITournamentRepository
{
    private readonly PhantomContext _context = context;

    public async Task<Tournament?> GetByIdAsync(Guid id)
    {
        return await _context.Tournaments
            .Include(t => t.Format)
            .Include(t => t.Organizer)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<IEnumerable<Tournament>> GetAllAsync()
    {
        return await _context.Tournaments
            .Include(t => t.Format)
            .Include(t => t.Organizer)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tournament>> GetByOrganizerAsync(Guid organizerId)
    {
        return await _context.Tournaments
            .Include(t => t.Format)
            .Include(t => t.Organizer)
            .Where(t => t.OrganizerId == organizerId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Tournament>> SearchAsync(TournamentSearchDto searchDto)
    {
        var query = _context.Tournaments
            .Include(t => t.Format)
            .Include(t => t.Organizer)
            .Where(t => t.IsActive);

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            query = query.Where(t => t.Name.Contains(searchDto.SearchTerm) ||
                                    t.Description.Contains(searchDto.SearchTerm));
        }

        if (!string.IsNullOrEmpty(searchDto.Status))
        {
            query = query.Where(t => t.Status == searchDto.Status);
        }

        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            query = query.Where(t => t.Location != null && t.Location.Contains(searchDto.Location));
        }

        if (!string.IsNullOrEmpty(searchDto.FormatId) && Guid.TryParse(searchDto.FormatId, out var formatId))
        {
            query = query.Where(t => t.FormatId == formatId);
        }

        if (searchDto.MinPrizePool.HasValue)
        {
            query = query.Where(t => t.PrizePool >= searchDto.MinPrizePool.Value);
        }

        if (searchDto.MaxPrizePool.HasValue)
        {
            query = query.Where(t => t.PrizePool <= searchDto.MaxPrizePool.Value);
        }

        if (searchDto.StartDateFrom.HasValue)
        {
            query = query.Where(t => t.StartDate >= searchDto.StartDateFrom.Value);
        }

        if (searchDto.StartDateTo.HasValue)
        {
            query = query.Where(t => t.StartDate <= searchDto.StartDateTo.Value);
        }

        if (searchDto.IsPublic.HasValue)
        {
            query = query.Where(t => t.IsPublic == searchDto.IsPublic.Value);
        }

        query = query.OrderByDescending(t => t.CreatedAt);

        if (searchDto.PageSize > 0)
        {
            query = query.Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                         .Take(searchDto.PageSize);
        }

        return await query.ToListAsync();
    }

    public async Task<PaginatedResponse<Tournament>> SearchWithPaginationAsync(TournamentSearchDto searchDto, Guid? userId = null)
    {
        var baseQuery = _context.Tournaments
            .Include(t => t.Format)
            .Include(t => t.Organizer)
            .Where(t => t.IsActive);

        var filteredQuery = baseQuery;

        if(userId.HasValue)
        {
            filteredQuery = filteredQuery.Where(t => t.OrganizerId == userId.Value);
        }

        if (!string.IsNullOrEmpty(searchDto.SearchTerm))
        {
            filteredQuery = filteredQuery.Where(t =>
                t.Name.Contains(searchDto.SearchTerm) ||
                t.Description.Contains(searchDto.SearchTerm));
        }

        if (!string.IsNullOrEmpty(searchDto.Status))
        {
            filteredQuery = filteredQuery.Where(t => t.Status == searchDto.Status);
        }

        if (!string.IsNullOrEmpty(searchDto.Location))
        {
            filteredQuery = filteredQuery.Where(t =>
                t.Location != null && t.Location.Contains(searchDto.Location));
        }

        if (!string.IsNullOrEmpty(searchDto.FormatId) && Guid.TryParse(searchDto.FormatId, out var formatId))
        {
            filteredQuery = filteredQuery.Where(t => t.FormatId == formatId);
        }

        if (searchDto.MinPrizePool.HasValue)
        {
            filteredQuery = filteredQuery.Where(t => t.PrizePool >= searchDto.MinPrizePool.Value);
        }

        if (searchDto.MaxPrizePool.HasValue)
        {
            filteredQuery = filteredQuery.Where(t => t.PrizePool <= searchDto.MaxPrizePool.Value);
        }

        if (searchDto.StartDateFrom.HasValue)
        {
            filteredQuery = filteredQuery.Where(t => t.StartDate >= searchDto.StartDateFrom.Value);
        }

        if (searchDto.StartDateTo.HasValue)
        {
            filteredQuery = filteredQuery.Where(t => t.StartDate <= searchDto.StartDateTo.Value);
        }

        if (searchDto.IsPublic.HasValue)
        {
            filteredQuery = filteredQuery.Where(t => t.IsPublic == searchDto.IsPublic.Value);
        }

        var totalRecords = await filteredQuery.CountAsync();

        var paginatedQuery = filteredQuery.OrderByDescending(t => t.CreatedAt);

        List<Tournament> tournaments;
        if (searchDto.PageSize > 0)
        {
            tournaments = await paginatedQuery
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();
        }
        else
        {
            tournaments = await paginatedQuery.ToListAsync();
        }

        return new PaginatedResponse<Tournament>(
            tournaments,
            searchDto.PageNumber,
            searchDto.PageSize > 0 ? searchDto.PageSize : totalRecords,
            totalRecords
        );
    }

    public async Task<Tournament> CreateAsync(Tournament tournament)
    {
        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task<Tournament> UpdateAsync(Tournament tournament)
    {
        var existing = await _context.Tournaments.FindAsync(tournament.Id);
        if (existing == null)
            throw new ArgumentException("Tournament not found.");

        tournament.UpdatedAt = DateTime.UtcNow;
        _context.Tournaments.Update(tournament);
        await _context.SaveChangesAsync();
        return tournament;
    }

    public async Task DeleteAsync(Guid id)
    {
        var tournament = await _context.Tournaments.FindAsync(id);
        if (tournament == null) return;

        _context.Tournaments.Remove(tournament);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Tournaments.AnyAsync(t => t.Id == id);
    }

    public async Task<bool> IsOrganizerAsync(Guid tournamentId, Guid userId)
    {
        return await _context.Tournaments
            .AnyAsync(t => t.Id == tournamentId && t.OrganizerId == userId);
    }

    public async Task<int> GetTeamCountAsync(Guid tournamentId)
    {
        return await _context.Teams
            .CountAsync(t => t.TournamentId == tournamentId && t.IsActive);
    }

    public async Task<int> GetApprovedTeamCountAsync(Guid tournamentId)
    {
        return await _context.Teams
            .CountAsync(t => t.TournamentId == tournamentId &&
                            t.RegistrationStatus == TeamRegistrationStatus.Approved.ToString() &&
                            t.IsActive);
    }
}
