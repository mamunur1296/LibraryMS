using LibraryMS.Application;
using LibraryMS.Application.Contracts.Services;
using LibraryMS.DbMigrator;
using LibraryMS.DbMigrator.Seeders;
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

        // Register Seeders
        services.AddTransient<IDataSeeder, BranchSeeder>();
        services.AddTransient<IDataSeeder, AuthorSeeder>();
        services.AddTransient<IDataSeeder, CategorySeeder>();
        services.AddTransient<IDataSeeder, BookSeeder>();
        services.AddTransient<IDataSeeder, UserAndMemberSeeder>();
        services.AddTransient<IDataSeeder, BorrowSeeder>();
        services.AddTransient<IDataSeeder, ReservationSeeder>();

        services.AddHostedService<DbMigratorHostedService>();
    })
    .Build();

await host.RunAsync();
