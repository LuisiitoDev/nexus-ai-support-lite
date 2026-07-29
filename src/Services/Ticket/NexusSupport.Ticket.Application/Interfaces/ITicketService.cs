using NexusSupport.Ticket.Application.Dtos;

namespace NexusSupport.Ticket.Application.Interfaces
{
    public interface ITicketService
    {
        Task<IReadOnlyList<TicketDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TicketDto?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken = default);
        Task<TicketDto> CreateAsync(TicketDto ticket, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TicketDto ticket, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid ticketId, CancellationToken cancellationToken = default);
    }
}
