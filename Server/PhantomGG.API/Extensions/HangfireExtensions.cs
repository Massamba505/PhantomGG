using Hangfire;
using Hangfire.Dashboard;
using Hangfire.SqlServer;
using PhantomGG.Service.Domain.Tournaments.Interfaces;

namespace PhantomGG.API.Extensions;

public static class HangfireExtensions
{
    public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PhantomDb");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string 'PhantomDb' is required for Hangfire");
        }

        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        services.AddHangfireServer(options =>
        {
            options.ServerName = Environment.MachineName + "-phantomgg";
            options.WorkerCount = 1;
        });

        return services;
    }

    public static void ConfigureHangfireJobs(this IApplicationBuilder app)
    {
        RecurringJob.AddOrUpdate<ITournamentBackgroundJobService>(
            "update-tournament-statuses",
            service => service.UpdateTournamentStatusesAsync(),
            "0 0 * * *",
            new RecurringJobOptions
            {
                TimeZone = TimeZoneInfo.Utc
            });
    }

    public static void UseHangfireDashboard(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AllowAllAuthorizationFilter() }
            });
        }
    }
}

public class AllowAllAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        return true;
    }
}
