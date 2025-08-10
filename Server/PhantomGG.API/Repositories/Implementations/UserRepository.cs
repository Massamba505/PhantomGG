using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PhantomGG.API.Data;
using PhantomGG.API.Models;
using PhantomGG.API.Repositories.Interfaces;

namespace PhantomGG.API.Repositories.Implementations;

/// <summary>
/// Implementation of the user repository
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the UserRepository
    /// </summary>
    /// <param name="userManager">User manager</param>
    /// <param name="context">Database context</param>
    public UserRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    /// <inheritdoc />
    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    /// <inheritdoc />
    public async Task AddAsync(ApplicationUser user)
    {
        await _userManager.CreateAsync(user);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(ApplicationUser user)
    {
        await _userManager.UpdateAsync(user);
    }

    /// <inheritdoc />
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }
}