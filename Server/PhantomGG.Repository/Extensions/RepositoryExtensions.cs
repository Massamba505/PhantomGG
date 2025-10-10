using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Implementations;
using PhantomGG.Repository.Interfaces;

namespace PhantomGG.Repository.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddApplicationDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PhantomContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("PhantomDb")));
        return services;
    }

    public static IServiceCollection AddApplicationRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<ITournamentTeamRepository, TournamentTeamRepository>();
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IMatchEventRepository, MatchEventRepository>();
        services.AddScoped<ITournamentStandingRepository, TournamentStandingRepository>();

        return services;
    }
}
