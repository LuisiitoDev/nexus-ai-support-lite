using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class UserStatusConfiguration : IEntityTypeConfiguration<UserStatusModel>
{
    public void Configure(EntityTypeBuilder<UserStatusModel> builder)
    {
        builder.ToTable("UserStatus");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        builder.Property(u => u.Name)
            .HasColumnName("Name")
            .HasMaxLength(250)
            .IsRequired();
    }
}
