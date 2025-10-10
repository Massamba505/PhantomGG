using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PhantomGG.Common.Config;

namespace PhantomGG.Common.Extensions;

public static class CommonExtensions
{
    public static IServiceCollection AddApplicationSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<CookieSettings>(configuration.GetSection("CookieSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.AddRouting(options => options.LowercaseUrls = true);
        return services;
    }
}
