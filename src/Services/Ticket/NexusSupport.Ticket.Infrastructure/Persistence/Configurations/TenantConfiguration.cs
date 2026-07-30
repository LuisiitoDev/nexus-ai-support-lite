using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<TenantModel>
{
    public void Configure(EntityTypeBuilder<TenantModel> builder)
    {
        builder.ToTable("Tenant");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id).HasColumnName("Id");

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .HasColumnName("IsActive")
            .IsRequired();

        builder.Property(t => t.CreateAt)
            .HasDefaultValueSql("GETDATE()", "DF_Tenant");
    }
}
