# 🏦 ZuluVault - Enterprise Digital Wallet Engine

[![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-336791?logo=postgresql)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7+-DC382D?logo=redis)](https://redis.io/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> **Production-ready digital wallet backend showcasing enterprise-grade fintech engineering for South African markets.**

**ZuluVault** is a complete financial infrastructure platform built from the ground up with **Clean Architecture**, **Domain-Driven Design**, and **security-first principles**. This isn't a tutorial project—it's a fully functional wallet system ready for real-world deployment.

**Built to demonstrate:** Advanced .NET engineering • Financial domain expertise • Security-conscious development • Production-ready code quality

## ⚡ Why This Project Stands Out

### 🎯 **Not Your Typical Portfolio Project**

This is a **complete financial system** with the complexity and rigor you'd find in production banking software. Every design decision prioritizes security, data integrity, and scalability.

### 💪 **What Makes It Enterprise-Grade**

**Financial Integrity**

- ACID-compliant transactions with PostgreSQL
- Idempotent operations prevent duplicate charges
- Immutable audit trail for regulatory compliance
- Daily transfer limits with automatic resets
- Balance tracking with before/after verification

**Production Security**

- JWT authentication with refresh token rotation
- Role-based access control (User/Admin/SuperAdmin)
- Comprehensive audit logging for every operation
- Failed login tracking with automatic account lockout
- Secure wallet locking for fraud prevention

**Scalable Architecture**

- Clean Architecture with Domain-Driven Design
- CQRS pattern using MediatR for command/query separation
- Redis caching for high-performance reads
- Repository and Unit of Work patterns
- Async/await throughout for non-blocking operations

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

- .NET 8 SDK
- PostgreSQL 15+ (or use Docker)
- Redis 7+ (or use Docker)

### Fastest Setup (Docker)

```bash
# 1. Start database services
docker-compose -f docker/docker-compose.yml up -d

# 2. Restore dependencies
dotnet restore

# 3. Run database migrations
cd src/ZuluVault.Api
dotnet ef database update --project ../ZuluVault.Infrastructure

# 4. Start the API
dotnet run

# 5. Open Swagger UI
# Navigate to https://localhost:5001/swagger
```

### Test Drive the API

1. **Register a User** → Creates wallet automatically
2. **Login** → Get JWT token
3. **Check Wallet** → View your new wallet
4. **Fund Wallet** → Use admin endpoint (see testing guide)
5. **Transfer Money** → Send to another user

📖 **Full Testing Guide:** Open `ZuluVault-Testing-Guide.html` in your browser for an interactive, step-by-step testing experience.

## 🎓 Skills & Patterns Demonstrated

**Backend Engineering**

- RESTful API design with OpenAPI/Swagger
- Clean Architecture & Domain-Driven Design
- CQRS pattern with MediatR
- Repository & Unit of Work patterns
- Dependency Injection throughout

**Data & Persistence**

- Entity Framework Core with PostgreSQL
- Complex entity relationships & migrations
- Database indexing & query optimization
- Redis distributed caching
- Dapper for performance-critical queries

**Security & Authentication**

- JWT with RS256 signing
- Refresh token rotation
- ASP.NET Identity integration
- Role-based authorization
- Comprehensive audit logging

**Testing & Quality**

- Unit testing with xUnit
- FluentAssertions for readable tests
- Mocking with Moq
- 90%+ code coverage on domain layer

**DevOps & Deployment**

- Docker multi-stage builds
- Docker Compose orchestration
- GitHub Actions CI/CD
- Automated testing pipeline
- Security scanning with Trivy

## 🏗️ Technical Architecture

```
┌─────────────────────────────────────────────────┐
│         API Layer (Presentation)                 │
│  Controllers • JWT Auth • Swagger • Middleware  │
└───────────────────┬─────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────┐
│      Application Layer (Business Logic)         │
│   MediatR Commands/Queries • DTOs • Validators  │
└───────────────────┬─────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────┐
│         Domain Layer (Core Business)             │
│     Entities • Value Objects • Exceptions        │
└───────────────────┬─────────────────────────────┘
                    │
┌───────────────────▼─────────────────────────────┐
│    Infrastructure Layer (External Services)      │
│   EF Core • Redis • Repositories • JWT Service  │
└──────────────────────────────────────────────────┘
```

**Core Design Principles:**

- Separation of Concerns via layer isolation
- Domain logic independent of infrastructure
- Testable business rules without dependencies
- Commands and queries separated (CQRS)
- Repository pattern abstracts data access

## 📊 Key Features Breakdown

### 🔐 Authentication & Authorization

- User registration with automatic wallet creation
- Secure login with JWT token generation
- Token refresh mechanism for extended sessions
- Role-based access (User, Admin, SuperAdmin)
- Failed login tracking with account lockout

### 💰 Wallet Operations

- Automatic wallet creation on registration
- Unique wallet numbers: `ZV-YYYYMMDD-XXXXXX`
- Multi-currency support (ZAR, USD, EUR, GBP)
- Real-time balance tracking
- Wallet locking/unlocking for security

### 💸 Peer-to-Peer Transfers

- ACID-compliant atomic transfers
- Idempotent operations prevent duplicates
- Daily transfer limits with automatic reset
- Insufficient funds validation
- Linked transaction records for both parties

### 📝 Compliance & Auditing

- Immutable transaction ledger
- Comprehensive audit logs for all operations
- Balance verification (before/after tracking)
- Transaction reference numbers for tracking
- Failed transaction logging

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run with code coverage
dotnet test /p:CollectCoverage=true

# View test results
dotnet test --logger "console;verbosity=detailed"
```

**Test Coverage:**

- Wallet entity business logic (15+ tests)
- Credit/Debit operations with validation
- Transfer eligibility checks
- Daily limit enforcement
- Wallet locking mechanisms

## 📈 Project Statistics

| Metric                | Count                     |
| --------------------- | ------------------------- |
| **Total Files**       | 35+ source files          |
| **Lines of Code**     | ~3,500+ (excluding tests) |
| **API Endpoints**     | 12+ RESTful endpoints     |
| **Database Tables**   | 5 core entities           |
| **Test Coverage**     | 90%+ on domain layer      |
| **Design Patterns**   | 8+ implemented            |
| **Security Features** | 7+ mechanisms             |

## 📚 Documentation

This project includes comprehensive documentation for all use cases:

- **[README.md](README.md)** - You are here! Project overview and quick start
- **[ZuluVault-Testing-Guide.html](ZuluVault-Testing-Guide.html)** - Interactive testing guide with beautiful UI

## 🚀 Deployment Options

**Cloud Platforms:**

**Containerized:**

- ✅ Docker Compose (included)
- ✅ Kubernetes ready
- ✅ On-premise deployment

## 🔮 Future Enhancements

Potential additions to showcase additional skills:

- [ ] GraphQL API endpoint
- [ ] Real-time notifications (SignalR)
- [ ] Rate limiting middleware
- [ ] API versioning
- [ ] Event sourcing pattern
- [ ] Microservices decomposition
- [ ] Mobile wallet integration
- [ ] Blockchain transaction recording

## 🤝 Contributing

This is a portfolio project built to demonstrate enterprise-level software engineering. While it's not actively seeking contributions, feedback and suggestions are always welcome!

**If you're a recruiter or hiring manager:**

- Feel free to test the live deployment
- Review the code quality and architecture
- Check the commit history for development practices
- See the testing guide for functional completeness

## 📄 License

MIT License - This project is free to use, modify, and distribute.

## 👨‍💻 Author & Contact

**Tsaagane Obakeng Shepherd**

📧 Email: obakengtsaagane@gmail.com  
💼 LinkedIn: [LinkedIn](www.linkedin.com/in/obakeng-tsaagane-307544244)  
🌐 Portfolio: [Portfolio](https://obakengshepherd.netlify.app/)  
💻 GitHub: [GitHub](https://github.com/obakengshepherd)

---

<div align="center">

### 🏆 Why This Project Exists

**ZuluVault isn't just another CRUD API—it's a complete financial infrastructure platform built with the same rigor and attention to detail you'd find in production banking systems.**

This project demonstrates:

- ✅ **Enterprise Architecture** - Clean Architecture, DDD, CQRS
- ✅ **Financial Domain Expertise** - ACID compliance, idempotency, audit trails
- ✅ **Security Consciousness** - JWT, RBAC, comprehensive logging
- ✅ **Production Readiness** - Docker, CI/CD, testing, documentation

**Built to showcase world-class software engineering for fintech applications.**

**If you're hiring for senior backend engineers, this is what "production-ready" looks like.** 💪

---

⭐ **Star this repo** if you find it useful or impressive!  
🔗 **Fork it** to use as a foundation for your own fintech projects  
📣 **Share it** with others who appreciate quality engineering

---

_Built with precision, passion, and a commitment to excellence._

</div>
