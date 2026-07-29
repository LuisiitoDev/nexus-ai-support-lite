using NexusSupport.Ticket.Domain.Enums;

namespace NexusSupport.Ticket.Application.Dtos;

public class TicketDto
{
    public Guid TenantId { get; set; }
    public Guid TicketId { get; set; }
    public Guid AssignedUserId { get; set; }
    public int TopicId { get; set; }
    public required string Subject { get; set; }
    public required string EscalationSummary { get; set; }
    public required string Description { get; set; }
    public bool IsLocked { get; set; }
    public bool IsEscalated { get; set; }
    public TicketStatus Status { get; set; } = TicketStatus.Draft;
    public TickerPriority Priority { get; set; } = TickerPriority.Low;
    public IReadOnlyList<MessageDto> Messages { get; set; } = [];
    public required TopicDto Topic { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}