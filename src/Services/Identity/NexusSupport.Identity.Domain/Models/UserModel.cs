namespace NexusSupport.Identity.Domain.Models;

public class UserModel
{
    public Guid Id { get; set; }
    public required string ExternalSubjectId { get; set; }
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string DisplayName { get; set; }
    public short Status { get; set; }
    public DateTime LastLogin { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}
