using NexusSupport.Ticket.Application.Dtos;
using NexusSupport.Ticket.Application.Interfaces;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Application.Services;

public sealed class MessageService(IMessageRepository repository,
    IMapper<MessageModel, MessageDto> mapper) : IMessageService
{
    public async Task<IReadOnlyList<MessageDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => [.. (await repository.GetAllAsync(cancellationToken)).Select(mapper.Map)];

    public async Task<MessageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => (await repository.GetByIdAsync(id, cancellationToken)) is MessageModel model ? mapper.Map(model) : null;

    public async Task<MessageDto> CreateAsync(MessageDto message, CancellationToken cancellationToken = default)
        => mapper.Map(await repository.CreateAsync(mapper.Map(message), cancellationToken));

    public async Task<bool> UpdateAsync(MessageDto message, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(mapper.Map(message), cancellationToken);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);
}
