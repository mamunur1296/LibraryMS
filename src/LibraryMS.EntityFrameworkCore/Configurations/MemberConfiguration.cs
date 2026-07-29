using LibraryMS.Domain.MemberManagement.AggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FirstName).IsRequired().HasMaxLength(50);
        builder.Property(m => m.LastName).IsRequired().HasMaxLength(50);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Phone).IsRequired().HasMaxLength(20);
        builder.Property(m => m.MembershipNumber).IsRequired().HasMaxLength(20);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasIndex(m => m.Email).IsUnique();
        builder.HasIndex(m => m.MembershipNumber).IsUnique();

        builder.Property(m => m.RowVersion).IsRowVersion();
    }
}
