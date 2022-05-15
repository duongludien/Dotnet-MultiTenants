using System.Reflection;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiTenantsDemo.Api.Models;

namespace MultiTenantsDemo.Api.Data;

public class BloggingDbContext : DbContext, IMultiTenantDbContext
{
    public ITenantInfo TenantInfo { get; set; }
    public TenantMismatchMode TenantMismatchMode { get; set; }
    public TenantNotSetMode TenantNotSetMode { get; set; }

    public DbSet<Post> Posts { get; set; }

    public BloggingDbContext(DbContextOptions<BloggingDbContext> options, ITenantInfo tenantInfo) : base(options)
    {
        TenantInfo = tenantInfo;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(TenantInfo?.ConnectionString ?? "User ID=postgres;Password=NoPassword1;Host=localhost;Port=5432;Database=MultiTenantsDemo.Tenant1;");
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.ConfigureMultiTenant();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.EnforceMultiTenant();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        this.EnforceMultiTenant();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
}
