// src/ZuluVault.Infrastructure/Persistence/ApplicationDbContextFactory.cs
// This factory allows EF Core tools (migrations) to create DbContext at design-time
// without needing the full ASP.NET Core host / DI container.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ZuluVault.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Load configuration from the API project's appsettings.json
        // (since connection string lives there in typical Clean Arch)
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("../../appsettings.json", optional: false)
            .AddJsonFile("../../appsettings.Development.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        }

        builder.UseNpgsql(connectionString, npgsqlOptions =>
        {
            npgsqlOptions.CommandTimeout(60);
        });

        return new ApplicationDbContext(builder.Options);
    }
}