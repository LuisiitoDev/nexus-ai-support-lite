namespace NexusSupport.Ticket.Application.Interfaces;

public interface IMapper<T1, T2>
{
    T2 Map(T1 model);
    T1 Map(T2 dto);
}