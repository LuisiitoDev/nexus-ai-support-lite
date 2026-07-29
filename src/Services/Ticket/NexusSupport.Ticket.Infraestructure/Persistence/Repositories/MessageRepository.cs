using Microsoft.EntityFrameworkCore;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infraestructure.Persistence.Repositories;

public sealed class MessageRepository(NexusDbContext dbContext) : IMessageRepository
{
    public async Task<IReadOnlyList<MessageModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Set<MessageModel>().AsNoTracking().ToListAsync(cancellationToken);

    public async Task<MessageModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Set<MessageModel>().AsNoTracking().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);

    public async Task<MessageModel> CreateAsync(MessageModel message, CancellationToken cancellationToken = default)
    {
        if (message.Id == Guid.Empty)
        {
            message.Id = Guid.NewGuid();
        }

        if (message.CreateAt == default)
        {
            message.CreateAt = DateTime.Now;
        }

        await dbContext.Set<MessageModel>().AddAsync(message, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return message;
    }

    public async Task<bool> UpdateAsync(MessageModel message, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<MessageModel>().FirstOrDefaultAsync(m => m.Id == message.Id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.TicketId = message.TicketId;
        existing.UserId = message.UserId;
        existing.Content = message.Content;
        dbContext.Entry(existing).Property(m => m.CreateAt).CurrentValue = message.CreateAt == default ? existing.CreateAt : message.CreateAt;

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<MessageModel>().FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Set<MessageModel>().Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
