using Hangfire;
using Hangfire.PostgreSql;
using LibraryMS.Application.Contracts.Auth;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.Infrastructure.Auth;
using LibraryMS.Infrastructure.Caching;
using LibraryMS.Infrastructure.Email;
using LibraryMS.Infrastructure.Export;
using LibraryMS.Infrastructure.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using StackExchange.Redis;

namespace LibraryMS.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Auth services (Adapter Pattern)
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // 2. Email service (Adapter Pattern - wraps MailKit)
        services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
        services.AddScoped<IEmailService, MailKitEmailService>();

        // 3. Redis Cache service (Adapter Pattern - wraps StackExchange.Redis)
        var redisConfig = configuration.GetSection(RedisOptions.SectionName).Get<RedisOptions>()
            ?? new RedisOptions();
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConfig.Configuration));
        services.AddScoped<ICacheService, RedisCacheService>();

        // 4. Report Export service (Strategy Pattern - Excel + PDF)
        QuestPDF.Settings.License = LicenseType.Community; // Community license
        services.AddScoped<IReportExportService, ReportExportService>();

        // 5. Hangfire (Background Job Scheduler)
        services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(c =>
                c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

        services.AddHangfireServer(options =>
        {
            options.WorkerCount = 5;
            options.Queues = ["default", "outbox", "reports"];
        });

        // 6. Register Infrastructure-level background job classes for DI
        services.AddScoped<OutboxProcessorJob>();

        return services;
    }
}
