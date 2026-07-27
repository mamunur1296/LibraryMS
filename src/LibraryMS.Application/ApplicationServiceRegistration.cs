using FluentValidation;
using LibraryMS.Application.Mapping;
using LibraryMS.Application.Behaviours;
using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.ReservationManagement;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LibraryMS.Application;

/// <summary>Application layer DI registration.</summary>
public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(ApplicationServiceRegistration).Assembly;
        var contractsAssembly = typeof(Application.Contracts.Common.PagedResult<>).Assembly;

        // MediatR — register all handlers from Application assembly
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            // Pipeline behaviors (order matters)
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });



        // FluentValidation — scan all validators in Contracts assembly
        services.AddValidatorsFromAssembly(contractsAssembly);

        // Domain Services
        services.AddScoped<BookManager>();
        services.AddScoped<BranchManager>();
        services.AddScoped<MemberManager>();
        services.AddScoped<BorrowManager>();

        return services;
    }
}
