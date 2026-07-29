using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Domain.Interfaces
{
    public interface ITopicRepository
    {
        Task<IReadOnlyList<TopicModel>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TopicModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TopicModel> CreateAsync(TopicModel topic, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TopicModel topic, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
