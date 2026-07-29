using NexusSupport.Ticket.Application.Dtos;
using NexusSupport.Ticket.Application.Interfaces;
using NexusSupport.Ticket.Domain.Enums;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Application.Services;

public sealed class TicketService(ITicketRepository repository, ITopicRepository topicRepository) : ITicketService
{
    public async Task<IReadOnlyList<TicketDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<TicketDto?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(ticketId, cancellationToken));

    public async Task<TicketDto> CreateAsync(TicketDto ticket, CancellationToken cancellationToken = default)
    {
        var model = await Map(ticket, cancellationToken);
        return Map(await repository.CreateAsync(model, cancellationToken));
    }

    public async Task<bool> UpdateAsync(TicketDto ticket, CancellationToken cancellationToken = default)
    {
        var model = await Map(ticket, cancellationToken);
        return await repository.UpdateAsync(model, cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid ticketId, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(ticketId, cancellationToken);

    private static TicketDto Map(TicketModel model) => new()
    {
        TenantId = model.TenantId,
        TicketId = model.TicketId,
        AssignedUserId = model.AssignedUserId,
        TopicId = model.TopicId,
        Subject = model.Subject,
        EscalationSummary = model.EscalationSummary,
        Description = model.Description,
        IsLocked = model.IsLocked,
        IsEscalated = model.IsEscalated,
        Status = model.Status,
        Priority = model.Priority,
        Messages = model.Messages.Select(Map).ToList(),
        Topic = Map(model.Topic),
        CreateAt = model.CreateAt,
        UpdateAt = model.UpdateAt
    };

    private async Task<TicketModel> Map(TicketDto dto, CancellationToken cancellationToken)
    {
        var topic = dto.Topic.Id == 0
            ? await topicRepository.CreateAsync(Map(dto.Topic), cancellationToken)
            : await topicRepository.GetByIdAsync(dto.Topic.Id, cancellationToken) ?? throw new InvalidOperationException($"Topic {dto.Topic.Id} was not found.");

        return new TicketModel
        {
            TenantId = dto.TenantId,
            TicketId = dto.TicketId,
            AssignedUserId = dto.AssignedUserId,
            TopicId = topic.Id,
            Subject = dto.Subject,
            EscalationSummary = dto.EscalationSummary,
            Description = dto.Description,
            IsLocked = dto.IsLocked,
            IsEscalated = dto.IsEscalated,
            Status = dto.Status,
            Priority = dto.Priority,
            Messages = dto.Messages.Select(m => Map(m, dto.TicketId)).ToList(),
            Topic = topic,
            CreateAt = dto.CreateAt,
            UpdateAt = dto.UpdateAt
        };
    }

    private static TopicDto Map(TopicModel model) => new()
    {
        Id = model.Id,
        TenantId = model.TenantId,
        Name = model.Name,
        Description = model.Description,
        IsActive = model.IsActive,
        CreateAt = model.CreateAt,
        Owners = model.Owners.Select(owner => new TopicOwnerDto
        {
            TopicId = owner.TopicId,
            TenantId = owner.TenantId,
            UserId = owner.UserId
        }).ToList()
    };

    private static TopicModel Map(TopicDto dto) => new()
    {
        Id = dto.Id,
        TenantId = dto.TenantId,
        Name = dto.Name,
        Description = dto.Description,
        IsActive = dto.IsActive,
        CreateAt = dto.CreateAt,
        Owners = dto.Owners.Select(owner => new TopicOwnerModel
        {
            TopicId = owner.TopicId == 0 ? dto.Id : owner.TopicId,
            TenantId = owner.TenantId,
            UserId = owner.UserId,
            Topic = new TopicModel
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreateAt = dto.CreateAt,
                Owners = []
            }
        }).ToList()
    };

    private static MessageDto Map(MessageModel model) => new()
    {
        Id = model.Id,
        TicketId = model.TicketId,
        UserId = model.UserId,
        Content = model.Content,
        CreateAt = model.CreateAt
    };

    private static MessageModel Map(MessageDto dto, Guid ticketId) => new()
    {
        Id = dto.Id,
        TicketId = dto.TicketId == Guid.Empty ? ticketId : dto.TicketId,
        UserId = dto.UserId,
        Content = dto.Content,
        CreateAt = dto.CreateAt
    };

    private static TicketDto? MapNullable(TicketModel? model) => model is null ? null : Map(model);
}
