using LibraryMS.EntityFrameworkCore.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LibraryMS.EntityFrameworkCore.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Type).IsRequired().HasMaxLength(500);
        builder.Property(o => o.Content).IsRequired().HasColumnType("text");
        builder.Property(o => o.Error).HasMaxLength(2000);

        // Index for efficient polling: only fetch unprocessed, non-dead messages
        builder.HasIndex(o => new { o.ProcessedOn, o.RetryCount });
    }
}
