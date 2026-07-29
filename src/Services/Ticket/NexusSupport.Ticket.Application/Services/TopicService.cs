using NexusSupport.Ticket.Application.Dtos;
using NexusSupport.Ticket.Application.Interfaces;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Application.Services;

public sealed class TopicService(ITopicRepository repository) : ITopicService
{
    public async Task<IReadOnlyList<TopicDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => (await repository.GetAllAsync(cancellationToken)).Select(Map).ToList();

    public async Task<TopicDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => MapNullable(await repository.GetByIdAsync(id, cancellationToken));

    public async Task<TopicDto> CreateAsync(TopicDto topic, CancellationToken cancellationToken = default)
        => Map(await repository.CreateAsync(Map(topic), cancellationToken));

    public async Task<bool> UpdateAsync(TopicDto topic, CancellationToken cancellationToken = default)
        => await repository.UpdateAsync(Map(topic), cancellationToken);

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => await repository.DeleteAsync(id, cancellationToken);

    private static TopicDto Map(TopicModel model) => new()
    {
        Id = model.Id,
        TenantId = model.TenantId,
        Name = model.Name,
        Description = model.Description,
        IsActive = model.IsActive,
        CreateAt = model.CreateAt,
        Owners = model.Owners.Select(Map).ToList()
    };

    private static TopicModel Map(TopicDto dto) => new()
    {
        Id = dto.Id,
        TenantId = dto.TenantId,
        Name = dto.Name,
        Description = dto.Description,
        IsActive = dto.IsActive,
        CreateAt = dto.CreateAt,
        Owners = dto.Owners.Select(Map).ToList()
    };

    private static TopicOwnerDto Map(TopicOwnerModel model) => new()
    {
        TopicId = model.TopicId,
        TenantId = model.TenantId,
        UserId = model.UserId
    };

    private static TopicOwnerModel Map(TopicOwnerDto dto) => new()
    {
        TopicId = dto.TopicId,
        TenantId = dto.TenantId,
        UserId = dto.UserId,
        Topic = new TopicModel
        {
            Id = dto.TopicId,
            TenantId = dto.TenantId,
            Name = string.Empty,
            Description = string.Empty,
            IsActive = true,
            CreateAt = DateTime.Now,
            Owners = []
        }
    };

    private static TopicDto? MapNullable(TopicModel? model) => model is null ? null : Map(model);
}
