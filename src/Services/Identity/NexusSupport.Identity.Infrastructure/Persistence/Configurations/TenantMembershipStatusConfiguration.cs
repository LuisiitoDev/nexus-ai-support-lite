using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Constants;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class TenantMembershipStatusConfiguration : IEntityTypeConfiguration<TenantMembershipStatusModel>
{
    public void Configure(EntityTypeBuilder<TenantMembershipStatusModel> builder)
    {
        builder.ToTable("TenantMembershipStatus");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(t => t.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired();

        builder.HasData(
            new TenantMembershipStatusModel { Id = TenantMembershipStatusCodes.EnabledId, Name = TenantMembershipStatusCodes.Enabled },
            new TenantMembershipStatusModel { Id = TenantMembershipStatusCodes.DisabledId, Name = TenantMembershipStatusCodes.Disabled });
    }
}
