using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<TenantModel>
{
    public void Configure(EntityTypeBuilder<TenantModel> builder)
    {
        builder.ToTable("Tenant");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("Id");

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(t => t.IsActive)
            .HasColumnName("IsActive")
            .IsRequired();

        builder.Property(t => t.CreateAt)
            .HasColumnName("CreateAt")
            .HasDefaultValueSql("GETDATE()", "DF_Tenant_CreateAt");

        builder.Property(t => t.UpdateAt)
            .HasColumnName("UpdateAt")
            .HasDefaultValueSql("GETDATE()", "DF_Tenant_UpdateAt");
    }
}
