# 🏦 ZuluVault - Enterprise Digital Wallet Engine

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?logo=postgresql)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7+-DC382D?logo=redis)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)

**ZuluVault** is a production-grade, security-first digital wallet backend designed for South African fintechs, banks, and SMEs. Built with financial integrity, compliance, and enterprise scalability in mind.

## 🎯 Core Features

### Financial Operations
- ✅ Multi-currency wallet accounts (ZAR-primary)
- ✅ Secure balance management with ACID compliance
- ✅ P2P transfers with idempotency guarantees
- ✅ Transaction ledger & immutable audit trails
- ✅ Daily transaction limits & fraud detection

### Security & Compliance
- 🔐 JWT authentication with refresh tokens
- 🛡️ Role-Based Access Control (RBAC)
- 📝 Comprehensive audit logging
- 🔒 Encrypted sensitive data at rest
- 🚨 Real-time fraud detection patterns

### Architecture
- 🏗️ Clean Architecture (DDD principles)
- 📦 CQRS pattern with MediatR
- 🔄 Redis caching for high performance
- 🐳 Docker & Docker Compose ready
- 📊 OpenAPI/Swagger documentation

## 🏗️ Architecture Overview

```
┌─────────────────────────────────────────────────┐
│              API Layer (Controllers)             │
│         JWT Auth, Validation, Mapping            │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│          Application Layer (Use Cases)           │
│      Commands, Queries, DTOs, Validators        │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│           Domain Layer (Business Logic)          │
│    Entities, Value Objects, Domain Events       │
└────────────────┬────────────────────────────────┘
                 │
┌────────────────▼────────────────────────────────┐
│        Infrastructure Layer (Data Access)        │
│   PostgreSQL, Redis, External Services          │
└──────────────────────────────────────────────────┘
```

## 📁 Project Structure

```
ZuluVault/
├── src/
│   ├── ZuluVault.Api/              # API endpoints, middleware, auth
│   ├── ZuluVault.Application/      # Business logic, CQRS handlers
│   ├── ZuluVault.Domain/           # Entities, value objects, interfaces
│   └── ZuluVault.Infrastructure/   # Data access, external services
├── tests/
│   ├── ZuluVault.UnitTests/
│   └── ZuluVault.IntegrationTests/
├── docker/
│   ├── docker-compose.yml
│   └── Dockerfile
└── README.md
```

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 15+](https://www.postgresql.org/download/)
- [Redis 7+](https://redis.io/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (optional)

### Local Development Setup

#### Step 1: Clone and Navigate
```powershell
git clone https://github.com/yourusername/ZuluVault.git
cd ZuluVault
```

#### Step 2: Create Project Structure
```powershell
# Create solution
dotnet new sln -n ZuluVault

# Create projects
dotnet new webapi -n ZuluVault.Api -o src/ZuluVault.Api
dotnet new classlib -n ZuluVault.Application -o src/ZuluVault.Application
dotnet new classlib -n ZuluVault.Domain -o src/ZuluVault.Domain
dotnet new classlib -n ZuluVault.Infrastructure -o src/ZuluVault.Infrastructure
dotnet new xunit -n ZuluVault.UnitTests -o tests/ZuluVault.UnitTests

# Add projects to solution
dotnet sln add src/ZuluVault.Api/ZuluVault.Api.csproj
dotnet sln add src/ZuluVault.Application/ZuluVault.Application.csproj
dotnet sln add src/ZuluVault.Domain/ZuluVault.Domain.csproj
dotnet sln add src/ZuluVault.Infrastructure/ZuluVault.Infrastructure.csproj
dotnet sln add tests/ZuluVault.UnitTests/ZuluVault.UnitTests.csproj

# Add project references
dotnet add src/ZuluVault.Api reference src/ZuluVault.Application
dotnet add src/ZuluVault.Api reference src/ZuluVault.Infrastructure
dotnet add src/ZuluVault.Application reference src/ZuluVault.Domain
dotnet add src/ZuluVault.Infrastructure reference src/ZuluVault.Application
dotnet add tests/ZuluVault.UnitTests reference src/ZuluVault.Domain
dotnet add tests/ZuluVault.UnitTests reference src/ZuluVault.Application
```

#### Step 3: Install Dependencies
```powershell
# Navigate to API project
cd src/ZuluVault.Api
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package Swashbuckle.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.Console
dotnet add package Serilog.Sinks.File

# Application layer
cd ../ZuluVault.Application
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package FluentValidation.DependencyInjectionExtensions
dotnet add package AutoMapper
dotnet add package AutoMapper.Extensions.Microsoft.DependencyInjection

# Infrastructure layer
cd ../ZuluVault.Infrastructure
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package StackExchange.Redis
dotnet add package Dapper
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

cd ../../..
```

#### Step 4: Database Setup
```powershell
# Using Docker (Recommended)
docker-compose -f docker/docker-compose.yml up -d

# OR Manual PostgreSQL setup
# Create database: zuluvault_db
# Update connection string in appsettings.json
```

#### Step 5: Run Migrations
```powershell
cd src/ZuluVault.Api
dotnet ef migrations add InitialCreate --project ../ZuluVault.Infrastructure
dotnet ef database update --project ../ZuluVault.Infrastructure
```

#### Step 6: Run the Application
```powershell
dotnet run --project src/ZuluVault.Api
```

Visit: `https://localhost:5001/swagger`

## 🐳 Docker Deployment

```powershell
# Build and run with Docker Compose
docker-compose -f docker/docker-compose.yml up --build

# Access API at: http://localhost:8080
# Swagger UI: http://localhost:8080/swagger
```

## 🔑 API Authentication

### Register User
```bash
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe",
  "phoneNumber": "+27821234567"
}
```

### Login
```bash
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123!"
}

# Response includes JWT token
{
  "token": "eyJhbGc...",
  "refreshToken": "...",
  "expiresIn": 3600
}
```

### Use Token
```bash
GET /api/wallets/my-wallet
Authorization: Bearer eyJhbGc...
```

## 💰 Core API Endpoints

### Wallet Management
- `GET /api/wallets/my-wallet` - Get user's wallet
- `GET /api/wallets/{id}/balance` - Check balance
- `GET /api/wallets/{id}/transactions` - Transaction history

### Transfers
- `POST /api/transfers` - Initiate P2P transfer
- `GET /api/transfers/{id}` - Get transfer status
- `GET /api/transfers/my-transfers` - User transfer history

### Admin Operations
- `POST /api/admin/wallets/{id}/credit` - Credit wallet (admin only)
- `POST /api/admin/wallets/{id}/debit` - Debit wallet (admin only)
- `GET /api/admin/audit-logs` - View audit logs

## 🧪 Testing

```powershell
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true /p:CoverageReportsFormat=opencover

# Run specific test project
dotnet test tests/ZuluVault.UnitTests
```

## 🛡️ Security Features

1. **Authentication & Authorization**
   - JWT with RS256 signing
   - Refresh token rotation
   - Role-based access control (User, Admin, SuperAdmin)

2. **Data Protection**
   - Sensitive data encryption at rest
   - SQL injection prevention via parameterized queries
   - XSS protection headers

3. **Audit & Compliance**
   - Immutable audit trail for all transactions
   - Failed login attempt tracking
   - IP address logging

4. **Rate Limiting**
   - Per-user transaction limits
   - API rate limiting middleware
   - Daily transfer caps

## 📊 Database Schema

### Core Tables
- `Users` - User accounts with ASP.NET Identity
- `Wallets` - Wallet accounts per user
- `Transactions` - Immutable transaction ledger
- `AuditLogs` - System-wide audit trail
- `TransferRequests` - P2P transfer tracking

## 🔧 Configuration

Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=zuluvault_db;Username=postgres;Password=yourpassword"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "Jwt": {
    "Secret": "your-256-bit-secret-key-here",
    "Issuer": "ZuluVault",
    "Audience": "ZuluVaultClients",
    "ExpiryMinutes": 60
  },
  "WalletSettings": {
    "DailyTransferLimit": 50000.00,
    "MinimumBalance": 0.00,
    "SupportedCurrencies": ["ZAR", "USD", "EUR"]
  }
}
```

## 📈 Performance

- Redis caching for frequently accessed data
- Database indexing on critical queries
- Connection pooling for PostgreSQL
- Async/await throughout the stack

## 🤝 Contributing

This is a portfolio project, but feedback and suggestions are welcome!

## 📄 License

MIT License - See LICENSE file for details

## 👨‍💻 Author

**Your Name**
- Portfolio: [yourportfolio.com]
- LinkedIn: [linkedin.com/in/yourprofile]
- GitHub: [@yourusername]

---

**Built with 💪 to showcase enterprise-grade fintech engineering capabilities**
