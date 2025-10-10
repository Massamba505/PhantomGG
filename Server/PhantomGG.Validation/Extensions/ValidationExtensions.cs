using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PhantomGG.Validation.Validators.Tournament;
using PhantomGG.Validation.Validators.Team;
using PhantomGG.Validation.Validators.Match;
using PhantomGG.Validation.Validators.User;

namespace PhantomGG.Validation.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddApplicationValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateTournamentValidator>();
        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Tournament.CreateTournamentDto>, CreateTournamentValidator>();
        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Tournament.UpdateTournamentDto>, UpdateTournamentValidator>();
        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Tournament.TournamentQuery>, TournamentQueryValidator>();

        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Team.CreateTeamDto>, CreateTeamValidator>();
        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Team.UpdateTeamDto>, UpdateTeamValidator>();
        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Team.TeamQuery>, TeamQueryValidator>();

        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Match.CreateMatchDto>, CreateMatchValidator>();
        services.AddScoped<IValidator<PhantomGG.Models.DTOs.Match.UpdateMatchDto>, UpdateMatchValidator>();

        services.AddScoped<IValidator<PhantomGG.Models.DTOs.User.UpdateUserProfileRequest>, UpdateUserProfileValidator>();
        services.AddScoped<IValidator<PhantomGG.Models.DTOs.User.ChangePasswordRequest>, ChangePasswordValidator>();

        return services;
    }
}
