using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class IdentityProviderConfiguration : IEntityTypeConfiguration<IdentityProviderModel>
{
    public void Configure(EntityTypeBuilder<IdentityProviderModel> builder)
    {
        builder.ToTable("IdentityProvider");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("Id");

        builder.Property(i => i.TenantId)
            .HasColumnName("TenantId")
            .IsRequired();

        builder.Property(i => i.ProviderType)
            .HasColumnName("ProviderType")
            .IsRequired();

        builder.Property(i => i.ClientId)
            .HasColumnName("ClientId")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(i => i.ClientSecret)
            .HasColumnName("ClientSecret")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(i => i.CallbackPath)
            .HasColumnName("CallbackPath")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(i => i.IsEnabled)
            .HasColumnName("IsEnabled")
            .IsRequired();

        builder.Property(i => i.CreateAt)
            .HasColumnName("CreateAt")
            .HasDefaultValueSql("GETDATE()", "DF_IdentityProvider_CreateAt");

        builder.Property(i => i.UpdateAt)
            .HasColumnName("UpdateAt")
            .HasDefaultValueSql("GETDATE()", "DF_IdentityProvider_UpdateAt");

        builder.HasOne<TenantModel>()
            .WithMany()
            .HasForeignKey(i => i.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<IdentityProviderTypeModel>()
            .WithMany()
            .HasForeignKey(i => i.ProviderType)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
