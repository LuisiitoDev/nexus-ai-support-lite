namespace NexusSupport.Ticket.Application.Dtos;

public class TenantDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsActive { get; set; }
    public DateTime CrateAt { get; set; }
}