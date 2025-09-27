using PhantomGG.API.Extensions;
using PhantomGG.API.Middleware;

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
        builder.Services.AddApplicationSettings(builder.Configuration);
        builder.Services.AddSwaggerDocumentation();
        builder.Services.AddCorsPolicy();
        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddApplicationRepositories();
        builder.Services.AddApplicationServices();

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.UseSwaggerDocumentation(app.Environment);
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCors("CorsPolicy");
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}