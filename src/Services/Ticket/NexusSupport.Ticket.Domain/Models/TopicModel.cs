namespace NexusSupport.Ticket.Domain.Models;

public class TopicModel
{
    public int Id { get; set; }
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateAt { get; set; }
    public required IReadOnlyList<TopicOwnerModel> Owners { get; set; } = [];
}