using FluentValidation;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;
using System.Text;
using ZuluVault.Api.Middleware;
using ZuluVault.Application.Features.Auth.Commands;
using ZuluVault.Application.Mapping;
using ZuluVault.Domain.Entities;
using ZuluVault.Domain.Interfaces;
using ZuluVault.Infrastructure.Persistence;
using ZuluVault.Infrastructure.Persistence.Repositories;
using ZuluVault.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// CONFIGURATION
// =====================================================

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT secret not configured");
var issuer = jwtSettings["Issuer"] ?? "ZuluVault";
var audience = jwtSettings["Audience"] ?? "ZuluVaultClients";
var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

// =====================================================
// LOGGING
// =====================================================

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/zuluvault-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// =====================================================
// SERVICES
// =====================================================

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure identity options
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
});

// Authentication - JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
        ClockSkew = TimeSpan.Zero
    };
    options.SaveToken = true;
});

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy =>
        policy.RequireRole("User", "Admin"));
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", corsPolicyBuilder =>
    {
        corsPolicyBuilder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("Redis:Configuration").Value;
});

// MediatR
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(RegisterCommand).Assembly);

// Domain Services
builder.Services.AddScoped<IJwtTokenService>(sp => new JwtTokenService(secret, issuer, audience, expiryMinutes));
builder.Services.AddScoped<ICacheService, RedisCacheService>();

// Repositories and Unit of Work
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// API Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ZuluVault API",
        Version = "v1",
        Description = "Enterprise Digital Wallet Engine for South African Fintechs",
        Contact = new OpenApiContact
        {
            Name = "ZuluVault Support",
            Email = "support@zuluvault.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT"
        }
    });

    // Add JWT authentication to Swagger
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { } }
    });

    // Include XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// =====================================================
// MIDDLEWARE PIPELINE
// =====================================================

// Database Migration
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (context.Database.IsSqlServer())
        {
            await context.Database.MigrateAsync();
        }
        else if (context.Database.IsNpgsql())
        {
            //await context.Database.MigrateAsync();
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error during database migration");
    }
}

// Exception Handling
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ZuluVault API v1");
        c.RoutePrefix = string.Empty;
    });
    app.UseDeveloperExceptionPage();
}

app.UseGlobalExceptionHandler();
app.UseRequestLogging();

// HTTPS Redirection
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Routing
app.UseRouting();

// Controllers
app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => new { status = "healthy", timestamp = DateTime.UtcNow })
    .WithName("Health")
    .WithOpenApi();

// =====================================================
// RUN
// =====================================================

try
{
    Log.Information("Starting ZuluVault API...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "ZuluVault API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
