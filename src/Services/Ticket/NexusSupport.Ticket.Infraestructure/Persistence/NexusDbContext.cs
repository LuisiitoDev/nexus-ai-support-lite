using Microsoft.EntityFrameworkCore;
using NexusSupport.Ticket.Application.interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Infraestructure.Persistence;

public class NexusDbContext(DbContextOptions<NexusDbContext> options, ITenantProvider provider) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NexusDbContext).Assembly);

        modelBuilder.Entity<MessageModel>().HasQueryFilter(m => m.TicketId == provider.TenantId);
        modelBuilder.Entity<TenantModel>().HasQueryFilter(m => m.Id == provider.TenantId);
        modelBuilder.Entity<TopicModel>().HasQueryFilter(m => m.TenantId == provider.TenantId);
    }
}