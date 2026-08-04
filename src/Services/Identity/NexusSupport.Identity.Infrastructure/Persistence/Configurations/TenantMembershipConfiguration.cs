using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembershipModel>
{
    public void Configure(EntityTypeBuilder<TenantMembershipModel> builder)
    {
        builder.ToTable("TenantMembership");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Id)
            .HasColumnName("Id")
            .HasDefaultValueSql("NEWID()", "DF_TenantMembership_Id");

        builder.Property(t => t.TenantId)
            .HasColumnName("TenantId")
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasColumnName("UserId")
            .IsRequired();

        // ADR-002 / DOMAIN_BOUNDARIES: a local user is scoped to exactly one Nexus organization,
        // so a user can have at most one membership regardless of tenant.
        builder.HasIndex(t => t.UserId)
            .IsUnique();

        builder.Property(t => t.Status)
            .HasColumnName("Status")
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .HasColumnName("CreatedAt")
            .HasDefaultValueSql("GETDATE()", "DF_TenantMembership_CreatedAt");

        builder.Property(t => t.JoinAt)
            .HasColumnName("JoinAt")
            .HasDefaultValueSql("GETDATE()", "DF_TenantMembership_JoinAt");

        builder.Property(t => t.UpdateAt)
            .HasColumnName("UpdateAt")
            .HasDefaultValueSql("GETDATE()", "DF_TenantMembership_UpdateAt");

        builder.HasOne<TenantModel>()
            .WithMany()
            .HasForeignKey(t => t.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<UserModel>()
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<TenantMembershipStatusModel>()
            .WithMany()
            .HasForeignKey(t => t.Status)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
