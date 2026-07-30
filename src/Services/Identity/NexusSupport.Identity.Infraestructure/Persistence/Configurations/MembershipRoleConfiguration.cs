using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infraestructure.Persistence.Configurations;

internal sealed class MembershipRoleConfiguration : IEntityTypeConfiguration<MembershipRoleModel>
{
    public void Configure(EntityTypeBuilder<MembershipRoleModel> builder)
    {
        builder.ToTable("MembershipRole");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Id)
            .HasColumnName("Id");

        builder.Property(m => m.TenantMembershipId)
            .HasColumnName("TenantMembershipId")
            .IsRequired();

        builder.Property(m => m.RoleId)
            .HasColumnName("RoleId")
            .IsRequired();

        builder.Property(m => m.CreateAt)
            .HasColumnName("CreateAt")
            .HasDefaultValueSql("GETDATE()", "DF_MembershipRole_CreateAt");

        builder.HasOne<TenantMembershipModel>()
            .WithMany()
            .HasForeignKey(m => m.TenantMembershipId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<RolModel>()
            .WithMany()
            .HasForeignKey(m => m.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
