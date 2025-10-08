using PhantomGG.API.Extensions;
using PhantomGG.API.Middleware;
using PhantomGG.Validation.Middleware;

namespace PhantomGG.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        // Configure application services
        builder.Services.AddApplicationDependencies(builder.Configuration);
        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddCorsPolicy();
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddRateLimiting();
        builder.Services.AddHybridCache();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.UseSwaggerDocumentation(app.Environment);
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors("CorsPolicy");
        app.UseMiddleware<ValidationMiddleware>();
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseRateLimiter();
        app.MapControllers();

        app.Run();
    }
}