using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(UserConsts.MaxUsernameLength);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(UserConsts.MaxEmailLength);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(UserConsts.MaxPasswordHashLength);
        builder.Property(u => u.PasswordSalt).IsRequired().HasMaxLength(UserConsts.MaxPasswordSaltLength);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(UserConsts.MaxRoleLength);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.HasOne<Member>().WithMany().HasForeignKey(u => u.MemberId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
        builder.HasOne<LibraryMS.Domain.BranchManagement.AggregateRoots.Branch>().WithMany().HasForeignKey(u => u.BranchId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
    }
}
