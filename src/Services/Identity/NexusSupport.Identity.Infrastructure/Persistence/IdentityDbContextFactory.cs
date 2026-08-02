using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace NexusSupport.Identity.Infrastructure.Persistence;

// Used only by `dotnet ef` tooling to build the model; never referenced at application runtime.
internal sealed class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    public IdentityDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=NexusIdentity;Trusted_Connection=True;TrustServerCertificate=True;");

        return new IdentityDbContext(optionsBuilder.Options);
    }
}
