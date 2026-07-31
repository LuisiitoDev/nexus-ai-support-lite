using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class IdentityProviderTypeConfiguration : IEntityTypeConfiguration<IdentityProviderTypeModel>
{
    public void Configure(EntityTypeBuilder<IdentityProviderTypeModel> builder)
    {
        builder.ToTable("IdentityProviderType");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(i => i.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired();
    }
}
