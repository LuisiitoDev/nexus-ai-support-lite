namespace NexusSupport.Identity.Application.Dtos;

public class IdentityProviderDto
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public short ProviderType { get; set; }
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string CallbackPath { get; set; }
    public bool IsEnabled { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime UpdateAt { get; set; }
}
