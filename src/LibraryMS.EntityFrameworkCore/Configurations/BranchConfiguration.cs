using LibraryMS.Domain.BranchManagement.AggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Name).IsRequired().HasMaxLength(100);
        builder.Property(b => b.Address).IsRequired().HasMaxLength(500);
        builder.Property(b => b.Phone).IsRequired().HasMaxLength(20);
        builder.Property(b => b.Email).IsRequired().HasMaxLength(100);
        
        builder.HasIndex(b => b.Name).IsUnique();
        builder.HasIndex(b => b.Email).IsUnique();
    }
}
