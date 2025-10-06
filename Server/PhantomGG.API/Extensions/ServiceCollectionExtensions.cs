using Microsoft.EntityFrameworkCore;
using PhantomGG.Repository.Data;
using PhantomGG.Repository.Implementations;
using PhantomGG.Repository.Interfaces;
using PhantomGG.Common.Config;
using PhantomGG.Service.Implementations;
using PhantomGG.Service.Interfaces;

namespace PhantomGG.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<CookieSettings>(configuration.GetSection("CookieSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddRouting(options => options.LowercaseUrls = true);
        return services;
    }

    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
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
        services.AddScoped<ITeamRepository, TeamRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<ITournamentStandingRepository, TournamentStandingRepository>();

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthVerificationService, AuthVerificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IMatchService, MatchService>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IImageService, LocalFileImageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();

        return services;
    }
}