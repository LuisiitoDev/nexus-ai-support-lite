using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infraestructure.Persistence.Configurations;

internal sealed class MessageConfiguration : IEntityTypeConfiguration<MessageModel>
{
    public void Configure(EntityTypeBuilder<MessageModel> builder)
    {
        builder.ToTable("Message");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id).HasColumnName("Id");

        builder.Property(m => m.TicketId)
            .HasColumnName("TicketId")
            .IsRequired();

        builder.Property(m => m.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        builder.Property(m => m.Content)
            .HasColumnName("Content")
            .HasMaxLength(10_0000)
            .IsRequired();

        builder.Property(m => m.CreateAt)
            .HasDefaultValueSql("GETDATE()","DF_Message");
    }
}