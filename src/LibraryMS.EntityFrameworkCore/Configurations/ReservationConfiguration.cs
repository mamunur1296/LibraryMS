using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.BranchManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.ReservationManagement.AggregateRoots;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne<Member>().WithMany().HasForeignKey(r => r.MemberId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Book>().WithMany().HasForeignKey(r => r.BookId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Branch>().WithMany().HasForeignKey(r => r.BranchId).OnDelete(DeleteBehavior.Restrict);
    }
}
