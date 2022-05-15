using Finbuckle.MultiTenant;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiTenantsDemo.Api.Data;
using MultiTenantsDemo.Api.Dtos;
using MultiTenantsDemo.Api.Models;

namespace MultiTenantsDemo.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly BloggingDbContext _bloggingDbContext;
    private readonly IMultiTenantStore<TenantInfo> _tenantStore;

    public TestController(BloggingDbContext bloggingDbContext, IMultiTenantStore<TenantInfo> tenantStore)
    {
        _bloggingDbContext = bloggingDbContext;
        _tenantStore = tenantStore;
    }

    [HttpGet("tenant")]
    public IActionResult GetTenantInfo()
    {
        var tenantInfo = HttpContext.GetMultiTenantContext<TenantInfo>().TenantInfo;
        return Ok(tenantInfo);
    }

    [HttpPost("post")]
    public IActionResult AddPost([FromBody] CreatePostDto request)
    {
        var post = new Post
        {
            Title = request.Title,
            Content = request.Content
        };

        _bloggingDbContext.Posts.Add(post);
        _bloggingDbContext.SaveChanges();

        return Ok(post);
    }

    [HttpPost("tenant")]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto request)
    {
        var tenants = await _tenantStore.GetAllAsync();
        if (tenants.All(tenant => tenant.Identifier != request.Identifier))
        {
            var tenantInfo = new TenantInfo
            {
                Id = Guid.NewGuid().ToString(),
                Identifier = request.Identifier,
                Name = request.Name,
                ConnectionString = request.ConnectionString
            };

            await _tenantStore.TryAddAsync(tenantInfo);
            var options = new DbContextOptionsBuilder<BloggingDbContext>();
            options.UseNpgsql(request.ConnectionString);
            var tenantDbContext = new BloggingDbContext(options.Options, tenantInfo);
            await tenantDbContext.Database.MigrateAsync();

            return Ok(tenantInfo);
        }

        return BadRequest();
    }
}
