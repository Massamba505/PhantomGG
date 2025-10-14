using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PhantomGG.Validation.Validators.Tournament;

namespace PhantomGG.Validation.Extensions;

public static class ValidationExtensions
{
    public static IServiceCollection AddApplicationValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateTournamentValidator>();
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;

        return services;
    }
}
