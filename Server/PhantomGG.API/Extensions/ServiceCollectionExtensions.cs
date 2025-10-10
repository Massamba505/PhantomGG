using PhantomGG.Common.Extensions;
using PhantomGG.Repository.Extensions;
using PhantomGG.Service.Extensions;
using PhantomGG.Validation.Extensions;

namespace PhantomGG.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationSettings(configuration);
        services.AddApplicationDatabase(configuration);
        services.AddApplicationRepositories();
        services.AddApplicationServices();
        services.AddApplicationValidation();

        return services;
    }
}