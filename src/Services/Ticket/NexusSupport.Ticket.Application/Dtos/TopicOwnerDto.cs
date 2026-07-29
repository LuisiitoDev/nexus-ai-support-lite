namespace NexusSupport.Ticket.Application.Dtos;

public class TopicOwnerDto
{
    public int TopicId { get; set; }
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
}