using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class RolConfiguration : IEntityTypeConfiguration<RolModel>
{
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
    }
}
