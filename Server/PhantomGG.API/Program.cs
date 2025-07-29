using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PhantomGG.API.Config;
using PhantomGG.API.Data;
using PhantomGG.API.Middleware;
using PhantomGG.API.Repositories.Implementations;
using PhantomGG.API.Repositories.Interfaces;
using PhantomGG.API.Services.Implementations;
using PhantomGG.API.Services.Interfaces;
using System.Text;

namespace PhantomGG.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        AddSwagger(builder.Services);
        ConfigureDatabase(builder.Services, builder.Configuration);
        ConfigureJwt(builder.Services, builder.Configuration);
        ConfigureServices(builder.Services);

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
        app.UseMiddleware<JwtMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

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

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: 'Authorization: Bearer {token}'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
    }

    private static void ConfigureDatabase(IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<PhantomGGContext>(options =>
            options.UseSqlServer(config.GetConnectionString("PhantomDb")));
    }

    private static void ConfigureJwt(IServiceCollection services, ConfigurationManager configuration)
    {
        services.Configure<JwtConfig>(configuration.GetSection("JwtConfig"));

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = configuration["JwtConfig:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["JwtConfig:Audience"],
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:Secret"]!)),
                ClockSkew = TimeSpan.Zero
            };
        });
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<ICookieService, CookieService>();
    }
}
