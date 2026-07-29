using Microsoft.EntityFrameworkCore;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infraestructure.Persistence.Repositories;

public sealed class TopicRepository(NexusDbContext dbContext) : ITopicRepository
{
    public async Task<IReadOnlyList<TopicModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Set<TopicModel>().AsNoTracking().Include(t => t.Owners).ToListAsync(cancellationToken);

    public async Task<TopicModel?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => await dbContext.Set<TopicModel>().AsNoTracking().Include(t => t.Owners).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<TopicModel> CreateAsync(TopicModel topic, CancellationToken cancellationToken = default)
    {
        if (topic.CreateAt == default)
        {
            topic.CreateAt = DateTime.Now;
        }

        topic.Owners ??= [];
        foreach (var owner in topic.Owners)
        {
            owner.Topic = topic;
            owner.TopicId = topic.Id;
        }

        await dbContext.Set<TopicModel>().AddAsync(topic, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return topic;
    }

    public async Task<bool> UpdateAsync(TopicModel topic, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<TopicModel>().Include(t => t.Owners).FirstOrDefaultAsync(t => t.Id == topic.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.TenantId = topic.TenantId;
        existing.Name = topic.Name;
        existing.Description = topic.Description;
        existing.IsActive = topic.IsActive;
        dbContext.Entry(existing).Property(t => t.CreateAt).CurrentValue = topic.CreateAt == default ? existing.CreateAt : topic.CreateAt;

        if (existing.Owners.Count > 0)
        {
            dbContext.RemoveRange(existing.Owners);
        }

        existing.Owners = topic.Owners?.Select(owner =>
        {
            owner.Topic = existing;
            owner.TopicId = existing.Id;
            return owner;
        }).ToList() ?? [];

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<TopicModel>().Include(t => t.Owners).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Set<TopicModel>().Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
