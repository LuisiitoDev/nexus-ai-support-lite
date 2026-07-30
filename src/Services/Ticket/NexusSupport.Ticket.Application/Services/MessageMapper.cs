using NexusSupport.Ticket.Application.Dtos;
using NexusSupport.Ticket.Application.Interfaces;
using NexusSupport.Ticket.Domain.Models;

namespace NexusSupport.Ticket.Application.Services.Mapper;

public class MessageMapper : IMapper<MessageModel, MessageDto>
{
    public MessageDto Map(MessageModel model) => new()
    {
        Id = model.Id,
        TicketId = model.TicketId,
        UserId = model.UserId,
        Content = model.Content,
        CreateAt = model.CreateAt
    };

    public MessageModel Map(MessageDto dto) => new()
    {
        Id = dto.Id,
        TicketId = dto.TicketId,
        UserId = dto.UserId,
        Content = dto.Content,
        CreateAt = dto.CreateAt
    };
}
