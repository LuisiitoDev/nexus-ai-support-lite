using NexusSupport.Ticket.Domain.Enums;

namespace NexusSupport.Ticket.Domain.Models;

public class TicketModel
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
    public TickerPriority Priority { get; set; }  = TickerPriority.Low;
    public IReadOnlyList<MessageModel> Messages { get; set; } = [];
    public required TopicModel Topic { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}