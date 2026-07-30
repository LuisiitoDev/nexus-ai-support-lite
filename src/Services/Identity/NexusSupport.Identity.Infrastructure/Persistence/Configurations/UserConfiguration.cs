using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserModel>
{
    public void Configure(EntityTypeBuilder<UserModel> builder)
    {
        builder.ToTable("User");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasColumnName("Id");

        builder.Property(u => u.Email)
            .HasColumnName("Email")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(u => u.FirstName)
            .HasColumnName("FirstName")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasColumnName("LastName")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(u => u.DisplayName)
            .HasColumnName("DisplayName")
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(u => u.Status)
            .HasColumnName("Status")
            .IsRequired();

        builder.Property(u => u.LastLogin)
            .HasColumnName("LastLogin");

        builder.Property(u => u.CreateAt)
            .HasColumnName("CreateAt")
            .HasDefaultValueSql("GETDATE()", "DF_User_CreateAt");

        builder.Property(u => u.UpdateAt)
            .HasColumnName("UpdateAt")
            .HasDefaultValueSql("GETDATE()", "DF_User_UpdateAt");
    }
}
