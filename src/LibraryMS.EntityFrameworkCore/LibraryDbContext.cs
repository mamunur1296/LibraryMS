using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BorrowManagement.Services;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.EntityFrameworkCore.Interceptors;
using LibraryMS.EntityFrameworkCore.Outbox;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore;

public sealed class LibraryDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableInterceptor;
    private readonly DomainEventToOutboxInterceptor _domainEventInterceptor;

    public LibraryDbContext(
        DbContextOptions<LibraryDbContext> options,
        AuditableEntityInterceptor auditableInterceptor,
        DomainEventToOutboxInterceptor domainEventInterceptor)
        : base(options)
    {
        _auditableInterceptor = auditableInterceptor;
        _domainEventInterceptor = domainEventInterceptor;
    }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookCopy> BookCopies => Set<BookCopy>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Member> Members => Set<Member>();

    public DbSet<BorrowRecord> BorrowRecords => Set<BorrowRecord>();
    public DbSet<Reservation> Reservations => Set<Reservation>();

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);

        // Apply Global Query Filter for Soft Delete
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(LibraryMS.Domain.Shared.Interfaces.ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, "IsDeleted");
                var condition = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(false));
                var lambda = System.Linq.Expressions.Expression.Lambda(condition, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor, _domainEventInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}

