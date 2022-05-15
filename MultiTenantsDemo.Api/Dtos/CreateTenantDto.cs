namespace MultiTenantsDemo.Api.Dtos;

public class CreateTenantDto
{
    public string Identifier { get; set; }
    public string Name { get; set; }
    public string ConnectionString { get; set; }
}
