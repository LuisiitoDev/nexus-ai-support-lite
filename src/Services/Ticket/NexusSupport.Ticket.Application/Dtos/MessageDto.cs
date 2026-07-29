namespace NexusSupport.Ticket.Application.Dtos;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public required string Content { get; set; }
    public DateTime CreateAt { get; set; }
}