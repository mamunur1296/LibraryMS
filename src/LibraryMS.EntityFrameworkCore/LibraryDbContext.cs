using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Entities;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using LibraryMS.Domain.SettingsManagement.Entities;
using LibraryMS.Domain.Shared.Interfaces;
using LibraryMS.EntityFrameworkCore.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LibraryMS.EntityFrameworkCore;

public sealed class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options)
        : base(options)
    {
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

    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, "IsDeleted");
                var condition = Expression.Equal(property, Expression.Constant(false));
                var lambda = Expression.Lambda(condition, parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
