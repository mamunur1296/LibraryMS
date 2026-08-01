using LibraryMS.Domain.BookManagement.AggregateRoots;
using LibraryMS.Domain.MemberManagement.AggregateRoots;
using LibraryMS.Domain.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class MemberConfiguration : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable("Members");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FirstName).IsRequired().HasMaxLength(MemberConsts.MaxFirstNameLength);
        builder.Property(m => m.LastName).IsRequired().HasMaxLength(MemberConsts.MaxLastNameLength);
        builder.Property(m => m.Email).IsRequired().HasMaxLength(MemberConsts.MaxEmailLength);
        builder.Property(m => m.Phone).IsRequired().HasMaxLength(MemberConsts.MaxPhoneLength);
        builder.Property(m => m.MembershipNumber).IsRequired().HasMaxLength(MemberConsts.MaxMembershipNumberLength);
        builder.Property(m => m.Status).HasConversion<string>().HasMaxLength(MemberConsts.MaxStatusLength);

        builder.HasIndex(m => m.Email).IsUnique();
        builder.HasIndex(m => m.MembershipNumber).IsUnique();

        builder.Property(m => m.RowVersion).IsConcurrencyToken();

        builder.OwnsMany(m => m.Favorites, fb =>
        {
            fb.ToTable("MemberFavorites");
            fb.HasKey(f => f.Id);
            fb.Property(f => f.Id).ValueGeneratedNever();
            fb.HasOne<Book>().WithMany().HasForeignKey(f => f.BookId);
        });
    }
}
