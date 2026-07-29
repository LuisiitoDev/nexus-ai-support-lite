using NexusSupport.Ticket.Application.Dtos;

namespace NexusSupport.Ticket.Application.Interfaces
{
    public interface ITopicService
    {
        Task<IReadOnlyList<TopicDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TopicDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TopicDto> CreateAsync(TopicDto topic, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(TopicDto topic, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
