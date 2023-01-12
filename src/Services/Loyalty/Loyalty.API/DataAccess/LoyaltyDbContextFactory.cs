using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Loyalty.API.DataAccess;

public class LoyaltyDbContextFactory : IDesignTimeDbContextFactory<LoyaltyContext>
{
    public LoyaltyContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
              .SetBasePath(Path.Combine(Directory.GetCurrentDirectory()))
              .AddJsonFile("appsettings.json")
              .AddEnvironmentVariables()
              .Build();

        var optionsBuilder = new DbContextOptionsBuilder<LoyaltyContext>();
        optionsBuilder.UseSqlServer(
            config["ConnectionString"], 
            sqlServerOptionsAction: o => o.MigrationsAssembly("Loyalty.API"));

        return new LoyaltyContext(optionsBuilder.Options);
    }
}
