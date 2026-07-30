using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TopicOwnerConfiguration : IEntityTypeConfiguration<TopicOwnerModel>
{
    public void Configure(EntityTypeBuilder<TopicOwnerModel> builder)
    {
        builder.ToTable("TopicOwner");

        builder.HasKey(o => new { o.TopicId, o.TenantId, o.UserId });

        builder.Property(o => o.TopicId)
            .HasColumnName("TopicId")
            .IsRequired();

        builder.Property(o => o.TenantId)
            .HasColumnName("TenantId")
            .IsRequired();

        builder.Property(o => o.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.HasOne(o => o.Topic)
            .WithMany(t => t.Owners)
            .HasForeignKey(o => o.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
