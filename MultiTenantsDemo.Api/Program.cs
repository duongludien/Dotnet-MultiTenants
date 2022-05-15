using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;
using MultiTenantsDemo.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMultiTenant<TenantInfo>()
    .WithHostStrategy()
    .WithEFCoreStore<MultiTenantStoreDbContext, TenantInfo>();

builder.Services
    .AddDbContext<BloggingDbContext>(options => options.ToString());

var app = builder.Build();


// Seed tenants
var scope = app.Services.CreateScope();

var multiTenantDbContext = scope.ServiceProvider.GetRequiredService<MultiTenantStoreDbContext>();
await multiTenantDbContext.Database.MigrateAsync();

var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<TenantInfo>>();
var tenants = (await tenantStore.GetAllAsync()).ToList();

const string tenant1Id = "4539f76f-614c-466f-a818-78dba0158380";
const string tenant1Identifier = "tenant1";
const string tenant1Name = "Tenant 1";
const string tenant1ConnectionString = "User ID=postgres;Password=NoPassword1;Host=localhost;Port=5432;Database=MultiTenantsDemo.Tenant1;";

const string tenant2Id = "2472c28a-b2b2-4879-906f-e49951a9b317";
const string tenant2Identifier = "tenant2";
const string tenant2Name = "Tenant 2";
const string tenant2ConnectionString = "User ID=postgres;Password=NoPassword1;Host=localhost;Port=5432;Database=MultiTenantsDemo.Tenant2;";

if (tenants.All(tenant => tenant.Id != tenant1Id))
{
    var tenant1Info = new TenantInfo
    {
        Id = tenant1Id,
        Identifier = tenant1Identifier,
        Name = tenant1Name,
        ConnectionString = tenant1ConnectionString
    };

    await tenantStore.TryAddAsync(tenant1Info);

    var options = new DbContextOptionsBuilder<BloggingDbContext>();
    options.UseNpgsql(tenant1ConnectionString);
    var tenant1DbContext = new BloggingDbContext(options.Options, tenant1Info);
    await tenant1DbContext.Database.MigrateAsync();
}

if (tenants.All(tenant => tenant.Id != tenant2Id))
{
    var tenant2Info = new TenantInfo
    {
        Id = tenant2Id,
        Identifier = tenant2Identifier,
        Name = tenant2Name,
        ConnectionString = tenant2ConnectionString
    };

    await tenantStore.TryAddAsync(tenant2Info);

    var options = new DbContextOptionsBuilder<BloggingDbContext>();
    options.UseNpgsql(tenant2ConnectionString);
    var tenant2DbContext = new BloggingDbContext(options.Options, tenant2Info);
    await tenant2DbContext.Database.MigrateAsync();
}




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMultiTenant();

// Tenant matter middlewares
app.UseAuthorization();
app.MapControllers();

app.Run();
