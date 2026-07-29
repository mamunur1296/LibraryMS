using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(250);
        builder.Property(u => u.PasswordSalt).IsRequired().HasMaxLength(250);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();
        
        builder.HasOne<Member>().WithMany().HasForeignKey(u => u.MemberId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);
    }
}
