using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration; 

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext> 
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

       var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseNpgsql(configuration.GetConnectionString("PostgresConnection"),
                b => b.MigrationsAssembly("StreamNest.Infrastructure"));

        return new RepositoryContext(builder.Options);

    }
}