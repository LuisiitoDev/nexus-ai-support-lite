using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using NexusSupport.Identity.Domain.Models;
using NexusSupport.Identity.Infrastructure.Persistence;

namespace NexusSupport.Identity.Infrastructure.Tests.Persistence;

/// <summary>
/// The production model's "Id"/"CreateAt"/"UpdateAt"/... defaults are SQL Server expressions
/// (NEWID(), GETDATE()) that SQLite does not understand, and SQLite cannot read back a
/// server-generated GUID default the way SQL Server's OUTPUT clause does. This test-only
/// subclass swaps the Guid keys to client-side generation and the SQL Server date defaults for
/// SQLite-compatible ones, so repository tests can exercise real INSERT/transaction behavior
/// against a lightweight relational engine instead of mocks.
/// </summary>
internal sealed class SqliteIdentityDbContext(DbContextOptions<IdentityDbContext> options) : IdentityDbContext(options)
{
    private const string NowSql = "(datetime('now'))";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TenantModel>(b =>
        {
            b.Property(t => t.Id).HasValueGenerator<GuidValueGenerator>();
            b.Property(t => t.CreateAt).HasDefaultValueSql(NowSql);
            b.Property(t => t.UpdateAt).HasDefaultValueSql(NowSql);
        });

        modelBuilder.Entity<UserModel>(b =>
        {
            b.Property(u => u.Id).HasValueGenerator<GuidValueGenerator>();
            b.Property(u => u.CreateAt).HasDefaultValueSql(NowSql);
            b.Property(u => u.UpdateAt).HasDefaultValueSql(NowSql);
        });

        modelBuilder.Entity<TenantMembershipModel>(b =>
        {
            b.Property(t => t.Id).HasValueGenerator<GuidValueGenerator>();
            b.Property(t => t.CreatedAt).HasDefaultValueSql(NowSql);
            b.Property(t => t.JoinAt).HasDefaultValueSql(NowSql);
            b.Property(t => t.UpdateAt).HasDefaultValueSql(NowSql);
        });

        modelBuilder.Entity<MembershipRoleModel>(b =>
        {
            b.Property(m => m.CreateAt).HasDefaultValueSql(NowSql);
        });
    }
}
