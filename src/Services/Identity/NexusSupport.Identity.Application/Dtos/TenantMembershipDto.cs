namespace NexusSupport.Identity.Application.Dtos;

public class TenantMembershipDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public short Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime JoinAt { get; set; }
    public DateTime UpdateAt { get; set; }
}
