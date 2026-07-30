namespace NexusSupport.Identity.Domain.Models
{
    public class MembershipRoleModel
    {
        public int Id { get; set; }
        public Guid TenantMembershipId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreateAt { get; set; }
    }
}