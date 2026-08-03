using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Constants;
using NexusSupport.Identity.Domain.Models;
using NexusSupport.Identity.Infrastructure.Persistence;
using NexusSupport.Identity.Infrastructure.Persistence.Repositories;
using NexusSupport.Identity.Infrastructure.Tests.Persistence;
using Xunit;

namespace NexusSupport.Identity.Infrastructure.Tests.Persistence.Repositories;

public class UserProvisioningRepositoryTests : IDisposable
{
    private const string EntraTenantId = "11111111-1111-1111-1111-111111111111";
    private const string Issuer = "https://login.microsoftonline.com/11111111-1111-1111-1111-111111111111/v2.0";
    private const string ExternalSubjectId = "external-subject-1";

    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly IdentityDbContext _dbContext;

    public UserProvisioningRepositoryTests()
    {
        _connection.Open();

        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseSqlite(_connection)
            .Options;

        _dbContext = new SqliteIdentityDbContext(options);
        _dbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task ProvisionAsync_ReturnsTenantNotFound_WhenNoTenantMatchesEntraTenantId()
    {
        var repository = new UserProvisioningRepository(_dbContext);

        var result = await repository.ProvisionAsync(
            "unknown-tenant", Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");

        Assert.Equal(ProvisioningOutcome.TenantNotFound, result.Outcome);
    }

    [Fact]
    public async Task ProvisionAsync_ReturnsTenantDisabled_WhenTenantIsNotActive()
    {
        await SeedTenantAsync(isActive: false);
        var repository = new UserProvisioningRepository(_dbContext);

        var result = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");

        Assert.Equal(ProvisioningOutcome.TenantDisabled, result.Outcome);
    }

    [Fact]
    public async Task ProvisionAsync_CreatesUserMembershipAndDefaultRequesterRole_OnFirstSignIn()
    {
        var tenant = await SeedTenantAsync(isActive: true);
        var repository = new UserProvisioningRepository(_dbContext);

        var result = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");

        Assert.Equal(ProvisioningOutcome.Created, result.Outcome);
        Assert.Equal(tenant.Id, result.TenantId);
        Assert.Equal([RoleCodes.Requester], result.RoleCodes);

        var user = await _dbContext.Users.AsNoTracking().SingleAsync(u => u.Id == result.UserId);
        Assert.Equal(UserStatusCodes.EnabledId, user.Status);
        Assert.Equal("user@example.com", user.Email);

        var membership = await _dbContext.TenantMemberships.AsNoTracking()
            .SingleAsync(m => m.TenantId == tenant.Id && m.UserId == user.Id);
        Assert.Equal(TenantMembershipStatusCodes.EnabledId, membership.Status);

        var membershipRole = await _dbContext.MembershipRoles.AsNoTracking()
            .SingleAsync(m => m.TenantMembershipId == membership.Id);
        Assert.Equal(RoleCodes.RequesterId, membershipRole.RoleId);
    }

    [Fact]
    public async Task ProvisionAsync_DoesNotCreateASecondMembership_WhenCalledTwiceForTheSameUser()
    {
        await SeedTenantAsync(isActive: true);
        var repository = new UserProvisioningRepository(_dbContext);

        var first = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");
        var second = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");

        Assert.Equal(ProvisioningOutcome.Created, first.Outcome);
        Assert.Equal(ProvisioningOutcome.Existing, second.Outcome);
        Assert.Equal(first.UserId, second.UserId);
        Assert.Equal(first.TenantMembershipId, second.TenantMembershipId);
        Assert.Equal(1, await _dbContext.TenantMemberships.CountAsync());
    }

    [Fact]
    public async Task ProvisionAsync_ReturnsExistingWithRolesAndSyncsProfile_WhenUserAlreadyProvisioned()
    {
        var tenant = await SeedTenantAsync(isActive: true);
        var (_, membership) = await SeedUserAsync(tenant.Id, UserStatusCodes.EnabledId, TenantMembershipStatusCodes.EnabledId);
        await SeedMembershipRoleAsync(membership.Id, RoleCodes.AdministratorId);

        var repository = new UserProvisioningRepository(_dbContext);

        var result = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "new-email@example.com", "Grace", "Hopper", "Grace Hopper");

        Assert.Equal(ProvisioningOutcome.Existing, result.Outcome);
        Assert.Equal([RoleCodes.Administrator], result.RoleCodes);
        Assert.Equal("new-email@example.com", result.Email);

        var user = await _dbContext.Users.AsNoTracking().SingleAsync(u => u.Id == result.UserId);
        Assert.Equal("new-email@example.com", user.Email);
        Assert.Equal("Grace", user.FirstName);
    }

    [Fact]
    public async Task ProvisionAsync_ReturnsUserDisabled_WhenUserIsLocallyDisabled()
    {
        var tenant = await SeedTenantAsync(isActive: true);
        await SeedUserAsync(tenant.Id, UserStatusCodes.DisabledId, TenantMembershipStatusCodes.EnabledId);
        var repository = new UserProvisioningRepository(_dbContext);

        var result = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");

        Assert.Equal(ProvisioningOutcome.UserDisabled, result.Outcome);
    }

    [Fact]
    public async Task ProvisionAsync_ReturnsMembershipDisabled_WhenMembershipIsDisabled()
    {
        var tenant = await SeedTenantAsync(isActive: true);
        await SeedUserAsync(tenant.Id, UserStatusCodes.EnabledId, TenantMembershipStatusCodes.DisabledId);
        var repository = new UserProvisioningRepository(_dbContext);

        var result = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");

        Assert.Equal(ProvisioningOutcome.MembershipDisabled, result.Outcome);
    }

    [Fact]
    public async Task ProvisionAsync_ReturnsTenantNotFound_WhenUserHasNoMembershipInResolvedTenant()
    {
        var tenant = await SeedTenantAsync(isActive: true);
        _dbContext.Users.Add(new UserModel
        {
            Issuer = Issuer,
            ExternalSubjectId = ExternalSubjectId,
            Email = "user@example.com",
            FirstName = "Ada",
            LastName = "Lovelace",
            DisplayName = "Ada Lovelace",
            Status = UserStatusCodes.EnabledId,
            LastLogin = DateTime.Now
        });
        await _dbContext.SaveChangesAsync();
        var repository = new UserProvisioningRepository(_dbContext);

        var result = await repository.ProvisionAsync(
            EntraTenantId, Issuer, ExternalSubjectId, "user@example.com", "Ada", "Lovelace", "Ada Lovelace");

        Assert.Equal(ProvisioningOutcome.TenantNotFound, result.Outcome);
        Assert.Equal(1, await _dbContext.Users.CountAsync());
    }

    private async Task<TenantModel> SeedTenantAsync(bool isActive)
    {
        var tenant = new TenantModel
        {
            Name = "Acme Corp",
            EntraTenantId = EntraTenantId,
            IsActive = isActive
        };
        _dbContext.Tenants.Add(tenant);
        await _dbContext.SaveChangesAsync();
        return tenant;
    }

    private async Task<(UserModel User, TenantMembershipModel Membership)> SeedUserAsync(
        Guid tenantId, short userStatus, short membershipStatus)
    {
        var user = new UserModel
        {
            Issuer = Issuer,
            ExternalSubjectId = ExternalSubjectId,
            Email = "old-email@example.com",
            FirstName = "Old",
            LastName = "Name",
            DisplayName = "Old Name",
            Status = userStatus,
            LastLogin = DateTime.Now
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        var membership = new TenantMembershipModel
        {
            TenantId = tenantId,
            UserId = user.Id,
            Status = membershipStatus,
            JoinAt = DateTime.Now
        };
        _dbContext.TenantMemberships.Add(membership);
        await _dbContext.SaveChangesAsync();

        return (user, membership);
    }

    private async Task SeedMembershipRoleAsync(Guid tenantMembershipId, int roleId)
    {
        _dbContext.MembershipRoles.Add(new MembershipRoleModel { TenantMembershipId = tenantMembershipId, RoleId = roleId });
        await _dbContext.SaveChangesAsync();
    }
}
