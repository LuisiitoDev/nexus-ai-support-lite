using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infraestructure.Persistence.Configurations;

internal sealed class TicketConfiguration : IEntityTypeConfiguration<TicketModel>
{
    public void Configure(EntityTypeBuilder<TicketModel> builder)
    {
        builder.ToTable("Ticket");

        builder.HasKey(t => t.TicketId);

        builder.Property(t => t.TenantId)
            .HasColumnName("TenantId")
            .IsRequired();

        builder.Property(t => t.TicketId)
            .HasColumnName("TicketId");

        builder.Property(t => t.AssignedUserId)
            .HasColumnName("AssignedUserId")
            .IsRequired();

        builder.Property(t => t.TopicId)
            .HasColumnName("TopicId")
            .IsRequired();

        builder.Property(t => t.Subject)
            .HasColumnName("Subject")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.EscalationSummary)
            .HasColumnName("EscalationSummary")
            .HasMaxLength(10_000)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasColumnName("Description")
            .HasMaxLength(10_000)
            .IsRequired();

        builder.Property(t => t.IsLocked)
            .HasColumnName("IsLocked")
            .IsRequired();

        builder.Property(t => t.IsEscalated)
            .HasColumnName("IsEscalated")
            .IsRequired();

        builder.Property(t => t.Status)
            .HasColumnName("Status")
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasColumnName("Priority")
            .IsRequired();

        builder.Property(t => t.CreateAt)
            .HasColumnName("CreateAt")
            .HasDefaultValueSql("GETDATE()", "DF_Ticket_CreateAt");

        builder.Property(t => t.UpdateAt)
            .HasColumnName("UpdateAt")
            .HasDefaultValueSql("GETDATE()", "DF_Ticket_UpdateAt");

        builder.HasOne(t => t.Topic)
            .WithMany()
            .HasForeignKey(t => t.TopicId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Messages)
            .WithOne()
            .HasForeignKey(m => m.TicketId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
