using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.ToTable("Books");
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Title).IsRequired().HasMaxLength(BookConsts.MaxTitleLength);
        builder.OwnsOne(b => b.ISBN, isbn =>
        {
            isbn.Property(i => i.Value).HasColumnName("ISBN").HasMaxLength(BookConsts.ISBNLength).IsRequired();
            isbn.HasIndex(i => i.Value).IsUnique();
        });
        
        builder.Property(b => b.Description).HasMaxLength(BookConsts.MaxDescriptionLength);
        builder.Property(b => b.Language).HasMaxLength(BookConsts.MaxLanguageLength);
        builder.Property(b => b.CoverImageUrl).HasMaxLength(BookConsts.MaxCoverImageUrlLength);
        
        // Relationships
        builder.HasOne<Category>().WithMany().HasForeignKey(b => b.CategoryId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Author>().WithMany().HasForeignKey(b => b.AuthorId).OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(b => b.RowVersion).IsConcurrencyToken();
    }
}
