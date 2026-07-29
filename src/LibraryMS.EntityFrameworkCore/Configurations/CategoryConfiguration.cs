using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(CategoryConsts.MaxNameLength);
        builder.HasIndex(c => c.Name).IsUnique();
        builder.Property(c => c.Description).HasMaxLength(CategoryConsts.MaxDescriptionLength);
    }
}
