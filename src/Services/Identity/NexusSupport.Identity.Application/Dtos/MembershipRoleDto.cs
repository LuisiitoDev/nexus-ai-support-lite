namespace NexusSupport.Identity.Application.Dtos;

public class MembershipRoleDto
{
    public int Id { get; set; }
    public Guid TenantMembershipId { get; set; }
    public int RoleId { get; set; }
    public DateTime CreateAt { get; set; }
}
