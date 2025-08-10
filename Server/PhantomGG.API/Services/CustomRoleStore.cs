using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using System.Threading;
using System.Threading.Tasks;

namespace PhantomGG.API.Services
{
    /// <summary>
    /// Custom role store that implements case-insensitive lookups without normalized fields
    /// </summary>
    public class CustomRoleStore : RoleStore<IdentityRole, ApplicationDbContext, string>
    {
        private readonly ApplicationDbContext _context;

        public CustomRoleStore(ApplicationDbContext context, IdentityErrorDescriber? describer = null)
            : base(context, describer)
        {
            _context = context;
        }

        /// <summary>
        /// Finds a role by name, using case-insensitive comparison
        /// </summary>
        public override async Task<IdentityRole?> FindByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            return await _context.Roles
                .FirstOrDefaultAsync(r => 
                    EF.Functions.Collate(r.Name, "SQL_Latin1_General_CP1_CI_AS") == normalizedName, 
                    cancellationToken);
        }

        /// <summary>
        /// We don't set normalized name since we don't have that field
        /// </summary>
        public override Task SetNormalizedNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken = default)
        {
            // Do nothing - we don't store normalized names
            return Task.CompletedTask;
        }

        /// <summary>
        /// We don't use concurrency stamps, so we can override this to just update without checking
        /// </summary>
        public override async Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            _context.Attach(role);
            _context.Update(role);
            
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Concurrency failure" });
            }
            
            return IdentityResult.Success;
        }
    }
}
