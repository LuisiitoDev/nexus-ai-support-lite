using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Constants;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class RolConfiguration : IEntityTypeConfiguration<RolModel>
{
    private static readonly DateTime SeedCreateAt = new(2026, 8, 1, 0, 0, 0, DateTimeKind.Utc);

    public void Configure(EntityTypeBuilder<RolModel> builder)
    {
        builder.ToTable("Rol");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("Id");

        builder.Property(r => r.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(r => r.Code)
            .HasColumnName("Code")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(r => r.Description)
            .HasColumnName("Description")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(r => r.IsActive)
            .HasColumnName("IsActive")
            .IsRequired();

        builder.Property(r => r.CreateAt)
            .HasColumnName("CreateAt")
            .HasDefaultValueSql("GETDATE()", "DF_Rol_CreateAt");

        // ADR-002: Requester, Agent, and Administrator are the only Nexus-managed product
        // roles for the MVP. Seeded so first-login provisioning can assign Requester by id.
        builder.HasData(
            new RolModel
            {
                Id = RoleCodes.RequesterId,
                Name = RoleCodes.Requester,
                Code = RoleCodes.Requester,
                Description = "Default role granted to every user on first valid sign-in. Creates and tracks their own incidents.",
                IsActive = true,
                CreateAt = SeedCreateAt
            },
            new RolModel
            {
                Id = RoleCodes.AgentId,
                Name = RoleCodes.Agent,
                Code = RoleCodes.Agent,
                Description = "Handles and resolves incidents assigned to their responsible topics.",
                IsActive = true,
                CreateAt = SeedCreateAt
            },
            new RolModel
            {
                Id = RoleCodes.AdministratorId,
                Name = RoleCodes.Administrator,
                Code = RoleCodes.Administrator,
                Description = "Manages organization users, roles, and account status.",
                IsActive = true,
                CreateAt = SeedCreateAt
            });
    }
}
