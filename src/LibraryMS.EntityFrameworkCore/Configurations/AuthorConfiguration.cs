using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.ToTable("Authors");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(AuthorConsts.MaxNameLength);
        builder.Property(a => a.Biography).HasMaxLength(AuthorConsts.MaxBiographyLength);
    }
}
