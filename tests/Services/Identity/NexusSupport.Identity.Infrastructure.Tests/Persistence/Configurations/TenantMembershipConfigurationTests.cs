using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Models;
using NexusSupport.Identity.Infrastructure.Persistence;
using NexusSupport.Identity.Infrastructure.Tests.Persistence;
using Xunit;

namespace NexusSupport.Identity.Infrastructure.Tests.Persistence.Configurations;

public class TenantMembershipConfigurationTests : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");
    private readonly IdentityDbContext _dbContext;

    public TenantMembershipConfigurationTests()
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
    public async Task SaveChanges_Throws_WhenTheSameUserGetsASecondMembershipInAnyTenant()
    {
        var user = new UserModel
        {
            Issuer = "https://login.microsoftonline.com/tenant/v2.0",
            ExternalSubjectId = "external-subject-1",
            Email = "user@example.com",
            FirstName = "Ada",
            LastName = "Lovelace",
            DisplayName = "Ada Lovelace",
            Status = 1,
            LastLogin = DateTime.Now
        };
        var firstTenant = new TenantModel { Name = "Acme Corp", EntraTenantId = "11111111-1111-1111-1111-111111111111", IsActive = true };
        var secondTenant = new TenantModel { Name = "Globex Corp", EntraTenantId = "22222222-2222-2222-2222-222222222222", IsActive = true };
        _dbContext.AddRange(user, firstTenant, secondTenant);
        await _dbContext.SaveChangesAsync();

        _dbContext.TenantMemberships.Add(new TenantMembershipModel { TenantId = firstTenant.Id, UserId = user.Id, Status = 1, JoinAt = DateTime.Now });
        await _dbContext.SaveChangesAsync();

        _dbContext.TenantMemberships.Add(new TenantMembershipModel { TenantId = secondTenant.Id, UserId = user.Id, Status = 1, JoinAt = DateTime.Now });

        await Assert.ThrowsAsync<DbUpdateException>(() => _dbContext.SaveChangesAsync());
    }
}
