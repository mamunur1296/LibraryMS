using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Token).IsRequired().HasMaxLength(250);

        builder.HasIndex(r => r.Token).IsUnique();

        builder.HasOne<User>().WithMany().HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
