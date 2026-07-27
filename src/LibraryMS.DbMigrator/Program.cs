using LibraryMS.Application;
using LibraryMS.DbMigrator;
using LibraryMS.EntityFrameworkCore;
using LibraryMS.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddApplication();
        services.AddEntityFrameworkCoreServices(hostContext.Configuration);
        services.AddInfrastructureServices(hostContext.Configuration);
        services.AddHostedService<DbMigratorHostedService>();
    })
    .Build();

await host.RunAsync();
