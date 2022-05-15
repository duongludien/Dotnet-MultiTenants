using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiTenantsDemo.Api.Models;

namespace MultiTenantsDemo.Api.ModelConfigs;

public class PostConfigs : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.IsMultiTenant();

        builder.Property(entity => entity.Id)
            .ValueGeneratedOnAdd();

        builder.HasKey(entity => entity.Id);
    }
}
