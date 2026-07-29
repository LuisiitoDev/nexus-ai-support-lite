using NexusSupport.Ticket.Application.Dtos;
using NexusSupport.Ticket.Application.Interfaces;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Application.Services;

public sealed class MessageService(IMessageRepository repository) : IMessageService
{
    public async Task<IReadOnlyList<MessageDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<MessageDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<MessageDto> CreateAsync(MessageDto message, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(message), cancellationToken));

    public async Task<bool> UpdateAsync(MessageDto message, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(message), cancellationToken);

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static MessageDto Map(MessageModel model) => new()
    {
        Id = model.Id,
        TicketId = model.TicketId,
        UserId = model.UserId,
        Content = model.Content,
        CreateAt = model.CreateAt
    };

    private static MessageModel Map(MessageDto dto) => new()
    {
        Id = dto.Id,
        TicketId = dto.TicketId,
        UserId = dto.UserId,
        Content = dto.Content,
        CreateAt = dto.CreateAt
    };

    private static MessageDto? MapNullable(MessageModel? model) => model is null ? null : Map(model);
}
