using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Repository.Entities;

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

        public async Task<int> GetPlayerCountByTeamAsync(Guid teamId)
        {
            return await _context.Players
                .CountAsync(p => p.TeamId == teamId);
        }
    }
}
