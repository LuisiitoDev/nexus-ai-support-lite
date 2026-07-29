using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<IReadOnlyList<MessageModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<MessageModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<MessageModel> CreateAsync(MessageModel message, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(MessageModel message, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
