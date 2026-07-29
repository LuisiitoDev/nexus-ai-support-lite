using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<IReadOnlyList<TicketModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TicketModel?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken = default);
        Task<TicketModel> CreateAsync(TicketModel ticket, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TicketModel ticket, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid ticketId, CancellationToken cancellationToken = default);
    }
}
