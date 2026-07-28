namespace NexusSupport.Ticket.Domain.Models;

public class TopicOwnerModel
{
    public int TopicId { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public required TopicModel Topic { get; set; }
}