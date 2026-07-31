using LibraryMS.Application;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.DbMigrator;
using LibraryMS.EntityFrameworkCore;
using LibraryMS.Infrastructure.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplication();
        services.AddEntityFrameworkCoreServices(hostContext.Configuration);

        // Only register what the migrator needs — skip Redis, Hangfire, Email, Export
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddHostedService<DbMigratorHostedService>();
    })
    .Build();

await host.RunAsync();
