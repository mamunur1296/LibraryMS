using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class BookCopyConfiguration : IEntityTypeConfiguration<BookCopy>
{
    public void Configure(EntityTypeBuilder<BookCopy> builder)
    {
        builder.ToTable("BookCopies");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CopyNumber).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne<Book>().WithMany(b => b.Copies).HasForeignKey(c => c.BookId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne<Branch>().WithMany().HasForeignKey(c => c.BranchId).OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.BookId, c.CopyNumber }).IsUnique();
    }
}
