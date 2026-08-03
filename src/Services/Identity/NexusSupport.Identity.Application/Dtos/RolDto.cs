namespace NexusSupport.Identity.Application.Dtos;

public class RolDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreateAt { get; set; }
}
