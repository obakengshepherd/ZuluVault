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
            .SetBasePath(Directory.GetCurrentDirectory())                  // Current dir = Api
            .AddJsonFile("../../appsettings.json", optional: false)       // Go up two levels to root, then to appsettings
            .AddJsonFile("../../appsettings.Development.json", optional: true)
            .Build();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found in appsettings.json");
        }

        builder.UseNpgsql(connectionString);  // <-- PostgreSQL provider

        // Optional: Enable detailed errors / logging during migrations
        // builder.EnableSensitiveDataLogging();
        // builder.EnableDetailedErrors();

        return new ApplicationDbContext(builder.Options);
    }
}