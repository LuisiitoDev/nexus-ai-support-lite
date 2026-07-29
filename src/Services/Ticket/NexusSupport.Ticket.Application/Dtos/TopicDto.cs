namespace NexusSupport.Ticket.Application.Dtos;

public class TopicDto
{
    public int Id { get; set; }
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateAt { get; set; }
    public required IReadOnlyList<TopicOwnerDto> Owners { get; set; } = [];
}