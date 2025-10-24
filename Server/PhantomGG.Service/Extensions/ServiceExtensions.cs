using Microsoft.Extensions.DependencyInjection;
using PhantomGG.Service.Auth.Implementations;
using PhantomGG.Service.Domain.Tournaments.Implementations;
using PhantomGG.Service.Domain.Matches.Implementations;
using PhantomGG.Service.Domain.Teams.Implementations;
using PhantomGG.Service.Domain.Users.Implementations;
using PhantomGG.Service.Domain.Users.Interfaces;
using PhantomGG.Service.Validation.Implementations;
using PhantomGG.Service.Infrastructure.Caching.Implementations;
using PhantomGG.Service.Infrastructure.Email.Implementations;
using PhantomGG.Service.Infrastructure.Storage.Implementations;
using PhantomGG.Service.Infrastructure.Security.Implementations;
using PhantomGG.Service.Domain.Tournaments.Interfaces;
using PhantomGG.Service.Infrastructure.Email.Interfaces;
using PhantomGG.Service.Infrastructure.Security.Interfaces;
using PhantomGG.Service.Infrastructure.Storage.Interfaces;
using PhantomGG.Service.Validation.Interfaces;
using PhantomGG.Service.Auth.Interfaces;
using PhantomGG.Service.Domain.Teams.Interfaces;
using PhantomGG.Service.Domain.Matches.Interfaces;
using PhantomGG.Service.Infrastructure.Caching.Interfaces;

namespace PhantomGG.Service.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAuthVerificationService, AuthVerificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<ITournamentTeamService, TournamentTeamService>();
        services.AddScoped<ITournamentStandingService, TournamentStandingService>();
        services.AddScoped<ITournamentValidationService, TournamentValidationService>();
        services.AddScoped<ITeamValidationService, TeamValidationService>();
        services.AddScoped<IMatchValidationService, MatchValidationService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IMatchService, MatchService>();
        services.AddScoped<IMatchEventService, MatchEventService>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ICookieService, CookieService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IImageService, LocalFileImageService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IEmailTemplateService, EmailTemplateService>();
        services.AddScoped<ICacheInvalidationService, CacheInvalidationService>();
        services.AddScoped<ITournamentBackgroundJobService, TournamentBackgroundJobService>();

        return services;
    }
}
