using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Name).IsRequired().HasMaxLength(BranchConsts.MaxNameLength);
        builder.Property(b => b.Address).IsRequired().HasMaxLength(BranchConsts.MaxAddressLength);
        builder.Property(b => b.Phone).IsRequired().HasMaxLength(BranchConsts.MaxPhoneLength);
        builder.Property(b => b.Email).IsRequired().HasMaxLength(BranchConsts.MaxEmailLength);
        
        builder.HasIndex(b => b.Name).IsUnique();
        builder.HasIndex(b => b.Email).IsUnique();
    }
}
