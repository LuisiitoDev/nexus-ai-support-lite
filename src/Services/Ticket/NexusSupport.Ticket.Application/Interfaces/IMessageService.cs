using NexusSupport.Ticket.Application.Dtos;

namespace NexusSupport.Ticket.Application.Interfaces
{
    public interface IMessageService
    {
        Task<IReadOnlyList<MessageDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<MessageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MessageDto> CreateAsync(MessageDto message, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(MessageDto message, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
