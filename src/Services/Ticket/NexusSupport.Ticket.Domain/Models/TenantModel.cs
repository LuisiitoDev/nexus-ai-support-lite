namespace NexusSupport.Ticket.Domain.Models;

public class TenantModel
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateAt { get; set; }
}