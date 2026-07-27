using LibraryMS.Domain.BookManagement;
using LibraryMS.Domain.BorrowManagement;
using LibraryMS.Domain.BranchManagement;
using LibraryMS.Domain.IdentityManagement;
using LibraryMS.Domain.MemberManagement;
using LibraryMS.Domain.ReservationManagement;
using LibraryMS.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.EntityFrameworkCore;

public sealed class LibraryDbContext : DbContext
{
    private readonly AuditableEntityInterceptor _auditableInterceptor;
    private readonly DomainEventDispatcherInterceptor _domainEventInterceptor;

    public LibraryDbContext(
        DbContextOptions<LibraryDbContext> options,
        AuditableEntityInterceptor auditableInterceptor,
        DomainEventDispatcherInterceptor domainEventInterceptor)
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LibraryDbContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_auditableInterceptor, _domainEventInterceptor);
        base.OnConfiguring(optionsBuilder);
    }
}
