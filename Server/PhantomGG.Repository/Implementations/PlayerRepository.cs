using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Models.Entities;
using PhantomGG.Models.DTOs.Player;

namespace PhantomGG.Repository.Implementations
{
    public class PlayerRepository(PhantomContext context) : IPlayerRepository
    {
        private readonly PhantomContext _context = context;

        public async Task<Player?> GetByIdAsync(Guid id)
        {
            return await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Player>> GetAllAsync()
        {
            return await _context.Players
                .Include(p => p.Team)
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Player>> GetByTeamAsync(Guid teamId)
        {
            return await _context.Players
                .Where(p => p.TeamId == teamId)
                .OrderBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<Player> CreateAsync(Player player)
        {
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task<Player> UpdateAsync(Player player)
        {
            _context.Players.Update(player);
            await _context.SaveChangesAsync();
            return player;
        }

        public async Task DeleteAsync(Guid id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Players.AnyAsync(p => p.Id == id);
        }

        public async Task<int> GetTeamPlayerCountAsync(Guid teamId)
        {
            return await _context.Players
                .CountAsync(p => p.TeamId == teamId);
        }

        public async Task<IEnumerable<Player>> GetByTournamentAsync(Guid tournamentId)
        {
            var teamIds = await _context.Teams
                .Select(t => t.Id)
                .ToListAsync();

            return await _context.Players
                .Include(p => p.Team)
                .OrderBy(p => p.Team.Name)
                .ThenBy(p => p.FirstName)
                .ThenBy(p => p.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Player>> SearchAsync(PlayerSearchDto searchDto)
        {
            var query = _context.Players
                .Include(p => p.Team)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                query = query.Where(p =>
                    p.FirstName.Contains(searchDto.SearchTerm) ||
                    p.LastName.Contains(searchDto.SearchTerm));
            }

            if (searchDto.TeamId.HasValue)
            {
                query = query.Where(p => p.TeamId == searchDto.TeamId.Value);
            }

            if (searchDto.Position.HasValue)
            {
                query = query.Where(p => p.Position == searchDto.Position.ToString());
            }

            return await query
                .Skip((searchDto.PageNumber - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();
        }

        public async Task<int> GetPlayerCountByTeamAsync(Guid teamId)
        {
            return await _context.Players
                .CountAsync(p => p.TeamId == teamId);
        }
    }
}
