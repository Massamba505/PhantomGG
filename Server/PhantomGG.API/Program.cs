using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using PhantomGG.API.Data;
using PhantomGG.API.Middleware;

namespace PhantomGG.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        AddSwagger(builder.Services);
        AddCors(builder.Services);
        ConfigureDatabase(builder.Services, builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PhantomGG API v1");
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseHttpsRedirection();
        app.UseCors("CorsPolicy");
        app.UseMiddleware<GlobalExceptionMiddleware>();
        app.MapControllers();

        // Apply EF Core migrations on startup
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            dbContext.Database.Migrate();
            Console.WriteLine("Applied Entity Framework Core migrations");
        }

        app.Run();
    }

    private static void AddSwagger(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PhantomGG API",
                Version = "v1",
                Description = "API for managing tournaments, teams, and players"
            });

            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    private static void AddCors(IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("PhantomDb")));
    }
}
