namespace NexusSupport.Ticket.Application.interfaces;

public interface ITenantProvider
{
    public Guid TenantId { get; }
}