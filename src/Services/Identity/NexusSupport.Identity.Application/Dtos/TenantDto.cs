namespace NexusSupport.Identity.Application.Dtos;

public class TenantDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string EntraTenantId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}
