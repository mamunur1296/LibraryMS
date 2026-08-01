using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BookManagement.Entities;
using LibraryMS.Domain.BorrowManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.IdentityManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class BorrowRecordConfiguration : IEntityTypeConfiguration<BorrowRecord>
{
    public void Configure(EntityTypeBuilder<BorrowRecord> builder)
    {
        builder.ToTable("BorrowRecords");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(BorrowRecordConsts.MaxStatusLength);
        builder.Property(r => r.LateFine).HasColumnType("decimal(18,2)");

        builder.HasOne<Member>().WithMany().HasForeignKey(r => r.MemberId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Book>().WithMany().HasForeignKey(r => r.BookId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<BookCopy>().WithMany().HasForeignKey(r => r.BookCopyId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(r => r.BranchId).OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(r => r.IssuedById)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(r => r.ReturnedById)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
