using Microsoft.EntityFrameworkCore;
using NexusSupport.Ticket.Domain.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infrastructure.Persistence.Repositories;

public sealed class TicketRepository(NexusDbContext dbContext) : ITicketRepository
{
    public async Task<IReadOnlyList<TicketModel>> GetAllAsync(CancellationToken cancellationToken = default)
        => await dbContext.Set<TicketModel>().AsNoTracking().Include(t => t.Topic).Include(t => t.Messages).ToListAsync(cancellationToken);

    public async Task<TicketModel?> GetByIdAsync(Guid ticketId, CancellationToken cancellationToken = default)
        => await dbContext.Set<TicketModel>().AsNoTracking().Include(t => t.Topic).Include(t => t.Messages).FirstOrDefaultAsync(t => t.TicketId == ticketId, cancellationToken);

    public async Task<TicketModel> CreateAsync(TicketModel ticket, CancellationToken cancellationToken = default)
    {
        if (ticket.TicketId == Guid.Empty)
        {
            ticket.TicketId = Guid.NewGuid();
        }

        if (ticket.CreateAt == default)
        {
            ticket.CreateAt = DateTime.Now;
        }

        ticket.UpdateAt = DateTime.Now;

        if (ticket.Topic is not null && dbContext.Entry(ticket.Topic).State == EntityState.Detached)
        {
            dbContext.Attach(ticket.Topic);
        }

        foreach (var message in ticket.Messages)
        {
            if (message.Id == Guid.Empty)
            {
                message.Id = Guid.NewGuid();
            }

            if (message.CreateAt == default)
            {
                message.CreateAt = DateTime.Now;
            }

            message.TicketId = ticket.TicketId;
        }

        await dbContext.Set<TicketModel>().AddAsync(ticket, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ticket;
    }

    public async Task<bool> UpdateAsync(TicketModel ticket, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<TicketModel>().Include(t => t.Topic).Include(t => t.Messages).FirstOrDefaultAsync(t => t.TicketId == ticket.TicketId, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        existing.TenantId = ticket.TenantId;
        existing.AssignedUserId = ticket.AssignedUserId;
        existing.TopicId = ticket.TopicId;
        existing.Subject = ticket.Subject;
        existing.EscalationSummary = ticket.EscalationSummary;
        existing.Description = ticket.Description;
        existing.IsLocked = ticket.IsLocked;
        existing.IsEscalated = ticket.IsEscalated;
        existing.Status = ticket.Status;
        existing.Priority = ticket.Priority;
        existing.UpdateAt = DateTime.Now;

        if (ticket.Topic is not null)
        {
            if (dbContext.Entry(ticket.Topic).State == EntityState.Detached)
            {
                dbContext.Attach(ticket.Topic);
            }

            existing.Topic = ticket.Topic;
        }

        if (existing.Messages.Count > 0)
        {
            dbContext.RemoveRange(existing.Messages);
        }

        var messages = ticket.Messages?.ToList() ?? [];
        foreach (var message in messages)
        {
            if (message.Id == Guid.Empty)
            {
                message.Id = Guid.NewGuid();
            }

            if (message.CreateAt == default)
            {
                message.CreateAt = DateTime.Now;
            }

            message.TicketId = existing.TicketId;
        }

        existing.Messages = messages;
        if (messages.Count > 0)
        {
            await dbContext.Set<MessageModel>().AddRangeAsync(messages, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> DeleteAsync(Guid ticketId, CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.Set<TicketModel>().FirstOrDefaultAsync(t => t.TicketId == ticketId, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        dbContext.Set<TicketModel>().Remove(existing);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
