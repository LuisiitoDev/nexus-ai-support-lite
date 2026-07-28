namespace NexusSupport.Ticket.Domain.Models;

public class MessageModel
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public DateTime CreateAt { get; set; }
}