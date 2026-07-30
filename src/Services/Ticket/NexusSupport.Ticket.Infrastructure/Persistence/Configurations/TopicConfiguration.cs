using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TopicConfiguration : IEntityTypeConfiguration<TopicModel>
{
    public void Configure(EntityTypeBuilder<TopicModel> builder)
    {
        builder.ToTable("Topic");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("Id");

        builder.Property(t => t.TenantId)
            .HasColumnName("TenantId")
            .IsRequired();

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("Description")
            .HasMaxLength(10_000)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .HasColumnName("IsActive")
            .IsRequired();

        builder.Property(t => t.CreateAt)
            .HasColumnName("CreateAt")
            .HasDefaultValueSql("GETDATE()", "DF_Topic_CreateAt");

        builder.HasMany(t => t.Owners)
            .WithOne(o => o.Topic)
            .HasForeignKey(o => o.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
