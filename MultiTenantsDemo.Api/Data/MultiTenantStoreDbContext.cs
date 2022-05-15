using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Stores;
using Microsoft.EntityFrameworkCore;

namespace MultiTenantsDemo.Api.Data;

public class MultiTenantStoreDbContext : EFCoreStoreDbContext<TenantInfo>
{
    public MultiTenantStoreDbContext(DbContextOptions<MultiTenantStoreDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("User ID=postgres;Password=NoPassword1;Host=localhost;Port=5432;Database=MultiTenantsDemo;");
        base.OnConfiguring(optionsBuilder);
    }
}
