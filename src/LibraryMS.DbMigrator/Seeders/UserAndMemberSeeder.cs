using LibraryMS.Application.Contracts.Services;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Enums;
using LibraryMS.EntityFrameworkCore;

namespace LibraryMS.DbMigrator.Seeders;

public class UserAndMemberSeeder : IDataSeeder
{
    private readonly IPasswordHasher _passwordHasher;

    public UserAndMemberSeeder(IPasswordHasher passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public async Task SeedAsync(LibraryDbContext dbContext, CancellationToken cancellationToken)
    {
        // 1. Members
        var memberJohn = new Member(Guid.NewGuid(), "John", "Doe", "john.doe@email.com", "555-1001", "LIB-2026-00001", "123 Main St, New York, NY");
        var memberJane = new Member(Guid.NewGuid(), "Jane", "Smith", "jane.smith@email.com", "555-1002", "LIB-2026-00002", "456 Oak Ave, San Francisco, CA");
        var memberBob = new Member(Guid.NewGuid(), "Bob", "Johnson", "bob.johnson@email.com", "555-1003", "LIB-2026-00003", "789 Pine Rd, Chicago, IL");
        var members = new[] { memberJohn, memberJane, memberBob };
        dbContext.Members.AddRange(members);

        // 2. Users (3) — one per role
        var (adminHash, adminSalt) = _passwordHasher.Hash("Admin123!");
        var (libHash, libSalt) = _passwordHasher.Hash("Librarian123!");
        var (memberHash, memberSalt) = _passwordHasher.Hash("Member123!");

        var adminUser = new User(Guid.NewGuid(), "admin", "admin@library.com", adminHash, adminSalt, UserRole.Admin);
        var libUser = new User(Guid.NewGuid(), "librarian", "librarian@library.com", libHash, libSalt, UserRole.Librarian);
        var memberUser = new User(Guid.NewGuid(), "member", "member@library.com", memberHash, memberSalt, UserRole.Member, memberJohn.Id);

        dbContext.Users.AddRange(adminUser, libUser, memberUser);
    }
}
