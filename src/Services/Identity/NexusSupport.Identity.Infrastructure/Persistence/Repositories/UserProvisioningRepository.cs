using Microsoft.EntityFrameworkCore;
using NexusSupport.Identity.Domain.Constants;
using NexusSupport.Identity.Domain.Interfaces;
using NexusSupport.Identity.Domain.Models;

namespace NexusSupport.Identity.Infrastructure.Persistence.Repositories;

public sealed class UserProvisioningRepository(IdentityDbContext dbContext) : IUserProvisioningRepository
{
    public async Task<UserProvisioningResultModel> ProvisionAsync(
        string entraTenantId,
        string issuer,
        string externalSubjectId,
        string email,
        string firstName,
        string lastName,
        string displayName,
        CancellationToken cancellationToken = default)
    {
        var tenant = await dbContext.Tenants.AsNoTracking()
            .FirstOrDefaultAsync(t => t.EntraTenantId == entraTenantId, cancellationToken);

        if (tenant is null)
            return new UserProvisioningResultModel { Outcome = ProvisioningOutcome.TenantNotFound };

        if (!tenant.IsActive)
            return new UserProvisioningResultModel { Outcome = ProvisioningOutcome.TenantDisabled };

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Issuer == issuer && u.ExternalSubjectId == externalSubjectId, cancellationToken);

        if (user is null)
            return await CreateUserAsync(tenant.Id, issuer, externalSubjectId, email, firstName, lastName, displayName, cancellationToken);

        if (user.Status == UserStatusCodes.DisabledId)
            return new UserProvisioningResultModel { Outcome = ProvisioningOutcome.UserDisabled };

        var membership = await dbContext.TenantMemberships.AsNoTracking()
            .FirstOrDefaultAsync(m => m.TenantId == tenant.Id && m.UserId == user.Id, cancellationToken);

        // The user exists but never joined this organization: nothing here authorizes it, so treat
        // it the same as an unknown tenant rather than silently creating a second membership.
        if (membership is null)
            return new UserProvisioningResultModel { Outcome = ProvisioningOutcome.TenantNotFound };

        if (membership.Status == TenantMembershipStatusCodes.DisabledId)
            return new UserProvisioningResultModel { Outcome = ProvisioningOutcome.MembershipDisabled };

        var now = DateTime.Now;
        user.Email = email;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.DisplayName = displayName;
        user.LastLogin = now;
        user.UpdateAt = now;
        await dbContext.SaveChangesAsync(cancellationToken);

        return new UserProvisioningResultModel
        {
            Outcome = ProvisioningOutcome.Existing,
            UserId = user.Id,
            TenantId = tenant.Id,
            TenantMembershipId = membership.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            RoleCodes = await GetRoleCodesAsync(membership.Id, cancellationToken)
        };
    }

    private async Task<UserProvisioningResultModel> CreateUserAsync(
        Guid tenantId,
        string issuer,
        string externalSubjectId,
        string email,
        string firstName,
        string lastName,
        string displayName,
        CancellationToken cancellationToken)
    {
        var now = DateTime.Now;

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

        var user = new UserModel
        {
            Issuer = issuer,
            ExternalSubjectId = externalSubjectId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            DisplayName = displayName,
            Status = UserStatusCodes.EnabledId,
            LastLogin = now
        };
        await dbContext.Users.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        var membership = new TenantMembershipModel
        {
            TenantId = tenantId,
            UserId = user.Id,
            Status = TenantMembershipStatusCodes.EnabledId,
            JoinAt = now
        };
        await dbContext.TenantMemberships.AddAsync(membership, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await dbContext.MembershipRoles.AddAsync(new MembershipRoleModel
        {
            TenantMembershipId = membership.Id,
            RoleId = RoleCodes.RequesterId
        }, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        return new UserProvisioningResultModel
        {
            Outcome = ProvisioningOutcome.Created,
            UserId = user.Id,
            TenantId = tenantId,
            TenantMembershipId = membership.Id,
            Email = user.Email,
            DisplayName = user.DisplayName,
            RoleCodes = [RoleCodes.Requester]
        };
    }

    private async Task<IReadOnlyList<string>> GetRoleCodesAsync(Guid tenantMembershipId, CancellationToken cancellationToken)
        => await (
            from membershipRole in dbContext.MembershipRoles.AsNoTracking()
            join role in dbContext.Roles.AsNoTracking() on membershipRole.RoleId equals role.Id
            where membershipRole.TenantMembershipId == tenantMembershipId
            select role.Code).ToListAsync(cancellationToken);
}
