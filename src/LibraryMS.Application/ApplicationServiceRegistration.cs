using FluentValidation;
using LibraryMS.Application.Behaviours;
using LibraryMS.Application.Contracts.Common;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.Services;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement.Services;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Entities;
using LibraryMS.Domain.IdentityManagement.Services;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryMS.Application;

/// <summary>Application layer DI registration.</summary>
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationServiceRegistration).Assembly;
        var contractsAssembly = typeof(PagedResult<>).Assembly;

        // MediatR — register all handlers from Application assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            // Chain of Responsibility pipeline (order matters: Logging -> Retry -> Validation -> Handler)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });



        // FluentValidation — scan all validators in Contracts assembly
        services.AddValidatorsFromAssembly(contractsAssembly);

        // Domain Services
        services.AddScoped<BookManager>();
        services.AddScoped<AuthorManager>();
        services.AddScoped<CategoryManager>();
        services.AddScoped<BookCopyManager>();
        services.AddScoped<BranchManager>();
        services.AddScoped<MemberManager>();
        services.AddScoped<BorrowManager>();
        services.AddScoped<UserManager>();
        services.AddScoped<RefreshTokenManager>();
        services.AddSingleton<IGuidGenerator, GuidGenerator>();

        // Background Jobs
        services.AddScoped<BackgroundJobs.OverdueCheckJob>();
        services.AddScoped<BackgroundJobs.ReservationExpiryJob>();
        services.AddScoped<BackgroundJobs.DailyFineAccumulationJob>();
        services.AddScoped<BackgroundJobs.MembershipExpiryJob>();
        services.AddScoped<BackgroundJobs.DueDateReminderJob>();

        return services;
    }
}
